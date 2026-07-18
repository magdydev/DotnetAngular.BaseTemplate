# DotnetAngular.BaseTemplate

A reusable, production-ready base template for full-stack applications: a **.NET 8** API built with **Domain-Driven Design / Clean Architecture**, **SQL Server** via EF Core with **ASP.NET Core Identity**, and an **Angular 18** (standalone components) frontend with Signals, i18n (EN/AR), and dynamic branding.

This is a *starting point*, not a finished app. It ships with `BrandingSettings` (single-row settings aggregate) and Identity (users/roles) to demonstrate every layer end to end — copy that pattern for each new feature.

## Starting a new project from this template

Don't hand-edit every "BaseTemplate" reference. Run the PowerShell script instead:

```bash
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme          # rename everything to Acme.*
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme -WhatIf  # preview first, changes nothing
```

It renames every `BaseTemplate` / `basetemplate` / `base-template` occurrence — namespaces, `.csproj`/`.sln` files and folders, the Angular package name, `appsettings.json`, `docker-compose.yml`, i18n strings — and regenerates the API project's `UserSecretsId` so it doesn't collide with the template's. It does **not** touch the "© MagdyTech Solutions" footer; that attribution is fixed on purpose. See the script's own comment-based help (`Get-Help ./scripts/New-ProjectFromTemplate.ps1 -Full`) for details.

## Solution layout

```
DotnetAngular.BaseTemplate/
├── backend/
│   ├── BaseTemplate.sln
│   ├── Directory.Build.props        # shared compiler settings (nullable, implicit usings, ...)
│   ├── global.json                  # pins the .NET SDK version
│   ├── src/
│   │   ├── BaseTemplate.Domain/         # entities, value objects, domain events, repository interfaces — no external dependencies
│   │   ├── BaseTemplate.Application/    # CQRS commands/queries, DTOs, validators, AutoMapper profiles
│   │   ├── BaseTemplate.Infrastructure/ # EF Core DbContext, Identity, repositories, UnitOfWork, migrations, seed
│   │   └── BaseTemplate.API/            # ASP.NET Core Web API, Swagger, JWT auth, middleware, DI composition root
│   └── tests/
│       └── BaseTemplate.Tests/          # xUnit tests for Domain and Application
├── frontend/                        # Angular 18 (standalone components, Signals, lazy-loaded features)
├── scripts/
│   └── New-ProjectFromTemplate.ps1  # rebrands backend + frontend for a new project (see above)
├── docker-compose.yml               # SQL Server + API for local development
└── README.md
```

### Why this structure

- **Domain** has zero package dependencies — it's pure C#. If you're tempted to add an EF Core or ASP.NET Core reference here, that logic belongs in Infrastructure or the API instead.
- **Application** orchestrates use cases (`ICommandHandler<,>` / `IQueryHandler<,>`) but never talks to a database directly — it depends only on repository *interfaces* defined in Domain.
- **Infrastructure** implements those interfaces with EF Core and SQL Server, and provides **ASP.NET Core Identity** (users & roles).
- **API** is the thin composition root: controllers translate HTTP to commands/queries via a small `IDispatcher`, nothing more.

## Backend

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local install, `docker compose up sqlserver`, or LocalDB on Windows)

### Run locally

```bash
cd backend
dotnet restore
dotnet build

# Create the initial migration (first time only)
dotnet ef migrations add InitialCreate \
  --project src/BaseTemplate.Infrastructure \
  --startup-project src/BaseTemplate.API

# Apply migrations — the API also does this automatically at startup in Development
dotnet ef database update \
  --project src/BaseTemplate.Infrastructure \
  --startup-project src/BaseTemplate.API

dotnet run --project src/BaseTemplate.API --urls https://localhost:5101
```

The API starts at `https://localhost:5101` (see `src/BaseTemplate.API/Properties/launchSettings.json`) with Swagger UI at `/swagger`. On first run in Development it creates:
- `BrandingSettings` row with defaults
- `Admin` role and an `admin` user with password `Admin@123`

> `dotnet ef` not installed? Run `dotnet tool install --global dotnet-ef` first.

### Configuration

Connection strings, JWT settings, CORS origins, and Serilog sinks all live in `src/BaseTemplate.API/appsettings.json`. **Never commit real secrets** — for anything beyond local development, use `dotnet user-secrets`, environment variables, or a secret manager, and override `Jwt:SigningKey` / `ConnectionStrings:DefaultConnection` there instead.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BaseTemplateDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "BaseTemplate.API",
    "Audience": "BaseTemplate.Client",
    "SigningKey": "REPLACE_WITH_A_SECRET_32_CHARS_OR_MORE",
    "ExpiryMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200", "http://localhost:4300", "http://localhost:4400"]
  }
}
```

**Important:** `Jwt:SigningKey` must be set to a real secret (32+ characters). The API will throw at startup if it's empty. The database password in `appsettings.json` is for local dev only — replace it in production.

### Authentication (ASP.NET Core Identity + JWT)

Users and roles are stored in the database via ASP.NET Core Identity. The `AuthController` (`POST /api/v1/auth/login`) validates credentials against `UserManager<AppUser>` and issues a JWT.

An admin user is seeded on first startup:
- **Username:** `admin`
- **Password:** `Admin@123`

Add `[Authorize]` to any controller/action to require authentication. The JWT includes `ClaimTypes.Role` claims for role-based authorization.

### Adding a new feature

1. **Domain**: add the entity/value object under `BaseTemplate.Domain/Entities` (or `ValueObjects`), plus an `I<Entity>Repository` interface if it needs custom queries.
2. **Infrastructure**: add an `IEntityTypeConfiguration<T>` under `Persistence/Configurations`, a repository implementation under `Persistence/Repositories`, and expose it on `IUnitOfWork`.
3. **Application**: add a feature folder (`Commands/<UseCase>`, `Queries/<UseCase>`) with a command/query, validator, and handler. DI registration is automatic — `DependencyInjection.AddApplication()` scans the assembly for handler implementations.
4. **API**: add a controller (or actions on an existing one) that calls `IDispatcher.Send(...)`.
5. **Tests**: mirror the pattern in `BaseTemplate.Tests/Domain` and `.../Application`.

### Tests

```bash
cd backend
dotnet test
```

## Frontend (Angular 18)

### Prerequisites

- Node.js 20+ and npm

### Run locally

```bash
cd frontend
npm install
npm start        # ng serve, http://localhost:4200
```

`npm run build` produces a production build in `dist/`. `npm run lint` runs ESLint, `npm run format` runs Prettier.

### Structure

```
src/app/
├── core/
│   ├── auth/              # auth.service.ts (login/logout/token), auth.guard.ts (CanActivate)
│   ├── interceptors/      # auth (attaches JWT), loading (global spinner), error (logging)
│   ├── models/            # BrandingSettings interface + logoSource() helper
│   └── services/          # branding, language (i18n + RTL), loading, health, toast, app-log
├── shared/
│   └── components/
│       ├── app-header/    # logo + brand name + language switcher + logout
│       ├── app-sidebar/   # nav links (settings)
│       ├── app-footer/    # "© MagdyTech Solutions" + logo
│       ├── global-spinner/ # full-screen loading overlay
│       └── toast/         # notification toasts (success/error/info)
├── features/
│   ├── auth/login/        # standalone login page (no shell layout)
│   └── settings/          # branding settings (name, logo, colors) — lazy-loaded, auth-guarded
├── app.config.ts          # providers: HTTP, router, i18n, interceptors, APP_INITIALIZER
└── app.routes.ts          # top-level route table
```

Add a new feature by creating `src/app/features/<feature>/` with its own `*.routes.ts` (lazy-loaded via `loadChildren`/`loadComponent`) and registering it in `app.routes.ts`.

The API base URL is set per environment in `src/environments/environment.ts` (dev) and `environment.production.ts` (prod).

### Design system

All visual tokens are CSS custom properties defined in `src/styles.scss`:

- **Colors**: OKLCH-based (`--color-primary`, `--color-secondary`, `--color-error`, etc.) — dynamically overridden by branding from the API
- **Typography**: Roboto (Latin) / Cairo (Arabic) fonts, auto-switched via `html[lang]`
- **Spacing**: `--space-1` through `--space-16` (4px base)
- **Radii**: `--radius-sm` through `--radius-xl`
- **Shadows**: `--shadow-sm` / `--shadow-md` / `--shadow-lg`
- **Motion**: `--ease-out` cubic-bezier, `--duration-fast/normal/slow`

Responsive breakpoints at `1024px`, `768px`, and `640px`. Sidebar collapses at `768px`. RTL via logical CSS properties (`inset-inline`, `border-inline-end`, `text-align: start`, `inset-inline-end`).

The toast component uses `inset-inline-end` for positioning and `:dir(rtl)` to swap the slide-in animation direction — toasts appear at top-right in English and top-left in Arabic automatically, sliding in from the nearest edge.

Accessibility: `prefers-reduced-motion`, `prefers-contrast: more`, `:focus-visible` outlines, ARIA roles on alerts, `ariaCurrentWhenActive` on nav.

### Branding (name / logo / colors — database-backed)

Name, logo, and colors are **not** build-time constants — they're stored in the database and served by `SettingsController` (`GET /api/v1/settings/branding`, `PUT /api/v1/settings/branding`).

- **Backend**: `BrandingSettings` is seeded at startup with defaults from `BrandingDefaults`. Colors validated as hex (`#RRGGBB` or `#RGB`) by both the domain entity and FluentValidation.
- **Frontend**: `BrandingService` fetches branding at startup via `APP_INITIALIZER`, then applies `--color-primary` / `--color-secondary` / `--color-hover` to `<html>`. Falls back to defaults if the API is unavailable.
- **Settings page** at `/settings` (auth-guarded): edit app name (EN/AR), upload logo (base64), pick colors — all saved via the API.

### Internationalization (English / Arabic)

Built with [`@ngx-translate/core`](https://github.com/ngx-translate/core). Translation files in `src/assets/i18n/en.json` and `ar.json`.

`LanguageService` manages the active language: persists to `localStorage`, sets `<html lang>`/`<html dir>` for automatic RTL via CSS direction — no separate RTL build. The header has an EN/AR switcher; call `languageService.use('ar' | 'en')` from anywhere.

Toast notifications (success/error/info) are also translated — keys live under the `TOAST` section in both locale files.

To add a third language: create `src/assets/i18n/<code>.json`, add `<code>` to `SUPPORTED_LANGUAGES` in `language.service.ts` (and to `RTL_LANGUAGES` if right-to-left), then add a button in the header.

### State management

- **Signals** used throughout for local component state and shared reactive state (auth, branding, language, loading, toasts)
- Services expose state as `signal()` / `computed()` with `asReadonly()` for public surfaces
- HTTP requests use `HttpClient` with `firstValueFrom()` + `async/await` in components
- **Global spinner** (`LoadingService` + interceptor): shown for all HTTP requests inside the authenticated shell
- **Toast notifications** (`ToastService` + `ToastComponent`): auto-dismissed after 4 seconds, color-coded by type, direction-aware (LTR top-right / RTL top-left)

## Docker (local SQL Server + API)

```bash
docker compose up --build
```

This starts SQL Server on `localhost:1433` and the API on `localhost:5100`. Run the Angular dev server separately with `npm start` in `frontend/` — its dev-server proxy is not wired into `docker-compose.yml` by design, so you get fast rebuilds while iterating on the UI.

## What's intentionally included

This template ships a complete foundation with:
- **`BrandingSettings`** — single-row settings aggregate demonstrating the full CRUD pattern
- **ASP.NET Core Identity** — users & roles in the database, admin user seeded
- **JWT authentication** — login endpoint issues tokens, auth guard protects routes
- **Settings UI** — edit app name, logo, colors; auth-guarded, lazy-loaded
- **Login page** — standalone, no shell layout, branded with dynamic logo/name
- **i18n** — English + Arabic with RTL, toast messages translated, footer copyright
- **Design system** — OKLCH tokens, responsive, accessible, dark-mode-ready

Copy whichever slice (Domain entity → EF configuration → repository → Application command/query → controller → Angular feature) fits the aggregate you're adding next.
