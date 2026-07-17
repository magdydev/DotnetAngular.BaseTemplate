# DotnetAngular.BaseTemplate

A reusable, production-ready base template for full-stack applications: a **.NET 8** API built with **Domain-Driven Design / Clean Architecture**, **SQL Server** via EF Core, and an **Angular** (standalone components) frontend.

This is a *starting point*, not a finished app. It ships with one sample aggregate (`Product`) that demonstrates every layer and pattern end to end — copy that pattern for each new feature.

## Starting a new project from this template

Don't hand-edit every "BaseTemplate" reference. Run the PowerShell script instead:

```bash
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme          # rename everything to Acme.*
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme -WhatIf  # preview first, changes nothing
```

It renames every `BaseTemplate` / `basetemplate` / `base-template` occurrence — namespaces, `.csproj`/`.sln` files and folders, the Angular package name, `appsettings.json`, `docker-compose.yml`, i18n strings — and regenerates the API project's `UserSecretsId` so it doesn't collide with the template's. It does **not** touch the "Powered by MagdyTech Solutions" footer; that attribution is fixed on purpose. See the script's own comment-based help (`Get-Help ./scripts/New-ProjectFromTemplate.ps1 -Full`) for details.

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
│   │   ├── BaseTemplate.Infrastructure/ # EF Core DbContext, repositories, UnitOfWork, migrations
│   │   └── BaseTemplate.API/            # ASP.NET Core Web API, Swagger, middleware, DI composition root
│   └── tests/
│       └── BaseTemplate.Tests/          # xUnit tests for Domain and Application
├── frontend/                        # Angular app (standalone components, lazy-loaded features)
├── scripts/
│   └── New-ProjectFromTemplate.ps1  # rebrands backend + frontend for a new project (see above)
├── docker-compose.yml                # SQL Server + API for local development
└── README.md
```

### Why this structure

- **Domain** has zero package dependencies — it's pure C#. If you're tempted to add an EF Core or ASP.NET Core reference here, that logic belongs in Infrastructure or the API instead.
- **Application** orchestrates use cases (`ICommandHandler<,>` / `IQueryHandler<,>`) but never talks to a database directly — it depends only on repository *interfaces* defined in Domain.
- **Infrastructure** implements those interfaces with EF Core and SQL Server.
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

dotnet run --project src/BaseTemplate.API
```

The API starts at `https://localhost:5101` (see `src/BaseTemplate.API/Properties/launchSettings.json`) with Swagger UI at `/swagger`. On first run in Development it seeds one sample product.

> `dotnet ef` not installed? Run `dotnet tool install --global dotnet-ef` first.

### Configuration

Connection strings, JWT settings, CORS origins, and Serilog sinks all live in `src/BaseTemplate.API/appsettings.json` / `appsettings.Development.json`. **Never commit real secrets** — for anything beyond local development, use `dotnet user-secrets`, environment variables, or a secret manager, and override `Jwt:SigningKey` / `ConnectionStrings:DefaultConnection` there instead.

### Adding a new feature

1. **Domain**: add the entity/value object under `BaseTemplate.Domain/Entities` (or `ValueObjects`), plus an `I<Entity>Repository` interface if it needs custom queries.
2. **Infrastructure**: add an `IEntityTypeConfiguration<T>` under `Persistence/Configurations`, a repository implementation under `Persistence/Repositories`, and expose it on `IUnitOfWork`.
3. **Application**: add a feature folder under `Products`-style (`Commands/<UseCase>`, `Queries/<UseCase>`) with a command/query, validator, and handler. DI registration is automatic — `DependencyInjection.AddApplication()` scans the assembly for handler implementations.
4. **API**: add a controller (or actions on an existing one) that calls `IDispatcher.Send(...)`.
5. **Tests**: mirror the pattern in `BaseTemplate.Tests/Domain` and `.../Application`.

### Authentication

JWT bearer validation is wired up (`Program.cs` + `Extensions/JwtSettings.cs`) and Swagger has an "Authorize" button ready to go, but **no token-issuing endpoint exists yet** — this is scaffolding, not a working login flow. Add a login/token endpoint (or point `Jwt:Issuer`/`Jwt:Audience` at an external identity provider) when you need real auth, and add `[Authorize]` to the controllers/actions that should require it.

### Tests

```bash
cd backend
dotnet test
```

## Frontend (Angular)

### Prerequisites

- Node.js 20+ and npm

### Run locally

```bash
cd frontend
npm install
npm start        # ng serve, http://localhost:4200
```

`npm run build` produces a production build in `dist/`. `npm test` runs the Karma/Jasmine unit tests, `npm run lint` runs ESLint, `npm run format` runs Prettier.

### Structure

```
src/app/
├── core/                 # singletons: HTTP services, interceptors, shared models
│   ├── interceptors/     # auth.interceptor.ts (attaches JWT), error.interceptor.ts (401/HTTP error handling)
│   ├── models/
│   └── services/         # language.service.ts (i18n + RTL), product.service.ts
├── shared/
│   └── components/
│       └── app-header/   # logo + brand name + language switcher
├── features/
│   └── products/         # sample feature: lazy-loaded via loadChildren, one standalone component
├── app.config.ts         # provideHttpClient, provideRouter, provideTranslateService, interceptor registration
└── app.routes.ts         # top-level route table
```

Add a new feature by creating `src/app/features/<feature>/` with its own `*.routes.ts` (lazy-loaded via `loadChildren`/`loadComponent`) and registering it in `app.routes.ts`. Services talk to the API through `HttpClient` and return Observables — consume them in templates with the `async` pipe (see `ProductListComponent`) rather than manual `subscribe()`.

The API base URL is set per environment in `src/environments/environment.ts` (dev) and `environment.production.ts` (prod build).

### Branding

- **Name**: "BaseTemplate" — set in `src/index.html` (`<title>`), `package.json`, and `assets/i18n/*.json` (`APP.NAME`). Handled automatically by `scripts/New-ProjectFromTemplate.ps1` (see above) when you fork this template.
- **Logo**: `src/assets/logo.svg` — a simple indigo/amber monogram, referenced from `AppHeaderComponent`. Swap the file for your own artwork; the header just points at the same path.
- **Colors**: defined once as CSS custom properties at the top of `src/styles.scss` (`--color-primary` #4F46E5 indigo, `--color-secondary` #F59E0B amber, plus text/background tokens). Change the values there and the whole app re-themes — nothing else references raw hex codes.
- **"Powered by" footer**: `AppFooterComponent` shows `src/assets/magdytech-logo.png` with a `FOOTER.POWERED_BY` translated caption. This is agency attribution, kept separate from the project's own name/logo/colors above — the rename script skips it deliberately.

### Internationalization (English / Arabic)

Built with [`@ngx-translate/core`](https://github.com/ngx-translate/core). Translation files live in `src/assets/i18n/en.json` and `ar.json`; add a key there and reference it in a template with the `translate` pipe (`{{ 'PRODUCTS.TITLE' | translate }}`, see `ProductListComponent`).

`LanguageService` (`core/services/language.service.ts`) owns the active language: it persists the choice to `localStorage`, and sets `<html lang>`/`<html dir>` so Arabic renders right-to-left automatically via native CSS direction — no separate RTL build. `AppHeaderComponent` has an EN/AR switcher wired to it; call `languageService.use('ar' | 'en')` from anywhere else you need to change language programmatically.

To add a third language: create `src/assets/i18n/<code>.json`, add `<code>` to `SUPPORTED_LANGUAGES` in `language.service.ts` (and to `RTL_LANGUAGES` too, if it's right-to-left), and add a button to `AppHeaderComponent`.

## Docker (local SQL Server + API)

```bash
docker compose up --build
```

This starts SQL Server on `localhost:1433` and the API on `localhost:5100`. Run the Angular dev server separately with `npm start` in `frontend/` — its dev-server proxy is not wired into `docker-compose.yml` by design, so you get fast rebuilds while iterating on the UI.

## What's intentionally left out

This template ships one sample entity (`Product`) and stops there — no CRUD for a second entity, no built-in login screen, no dashboard pages. Copy the `Product` slice (Domain entity → EF configuration → repository → Application command/query → controller → Angular feature) for every new aggregate you add.
