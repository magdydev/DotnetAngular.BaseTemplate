# Plans

## Current features

- **Auth**: ASP.NET Core Identity with JWT, login page, auth guard (lazy-loaded)
- **Branding**: dynamic app name/logo/colors, stored in DB, editable via settings page (lazy-loaded, auth-guarded)
- **User management**: list/create/edit (email + roles)/reset password/delete via `UsersController`, admin-only; self-delete and self-demote-from-Admin are blocked server-side
- **Role management**: role CRUD via `RolesController` — roles are DB rows, not a hardcoded enum, so a new role is immediately assignable to users with no code change or redeploy. The seeded `Admin` role can't be deleted
- **i18n**: English + Arabic with automatic RTL, translated toast messages
- **Design system**: OKLCH tokens, responsive layout (640/768/1024 breakpoints), accessible, dark-mode-ready
- **Toast notifications**: direction-aware (top-right LTR / top-left RTL), auto-dismiss, success/error/info
- **Confirm dialog**: `ConfirmService.confirm(...)` returns a `Promise<boolean>` — same call shape as the native `confirm()` it replaces, but renders as a translated, themeable modal (`ConfirmDialogComponent`). Wired into both delete flows (users, roles); reuse it for any other destructive action
- **Date picker**: `DatePickerComponent`, a custom calendar (not the native `<input type="date">`) implementing `ControlValueAccessor` — drop it into any form via `[(ngModel)]`/reactive forms. Model value is an ISO `yyyy-MM-dd` string; month/weekday labels come from `Intl.DateTimeFormat` so EN/AR both work with no translation-map maintenance; `minDate`/`maxDate` inputs disable out-of-range days
- **Shell**: header (brand + lang toggle + logout), collapsible sidebar, footer ("© MagdyTech Solutions" + logo), global spinner

## Architecture decisions

| Decision | Rationale |
|---|---|
| No `SignInManager` | `AddIdentityCore` keeps the DB schema lean; `UserManager.CheckPasswordAsync` does the same without SignInManager's cookie baggage |
| Signals everywhere | Lighter than RxJS Subjects for simple state; `computed` + `effect` replace manual subscriptions |
| Inline styles in components | Keeps toast/global-spinner truly standalone; avoids extra files for sub-20-line components |
| `firstValueFrom` + `async/await` | Simpler than `.subscribe()` + manual cleanup for one-shot HTTP calls |
| `inset-inline-end` for toasts | Native CSS logical property — no JS, no `[dir]` bindings; flips automatically |
| `:dir()` for animation | Selector-based RTL branching without duplicating elements or JS checks |
| `takeUntilDestroyed` | Angular 18 built-in; no need for manual `ngOnDestroy` + `Subject` pattern |
| Emulated ViewEncapsulation | Default, scopes styles without shadow DOM overhead |
| `APP_INITIALIZER` for branding | Blocks nothing; API failure falls back gracefully without breaking startup |
| `providedIn: 'root'` for all services | Tree-shakeable, no manual module registration |
| Users/Roles controllers call `UserManager`/`RoleManager` directly, skipping Application/CQRS | `AppUser` lives in Infrastructure; an Application-layer command referencing it would invert the dependency direction. Same choice `AuthController` already made — Identity is already a persistence abstraction, wrapping it again would just be ceremony |
| `FrameworkReference` to `Microsoft.AspNetCore.App` in Infrastructure | The Identity NuGet package (`Microsoft.Extensions.Identity.Core`) is missing `AddDefaultTokenProviders()` and the concrete token providers — they need the full ASP.NET Core shared framework, which a plain class library doesn't get automatically |
| Confirm dialog backdrop is a `<button>`, not a `div` with `(click)` | Native buttons are keyboard-operable and focusable for free; a clickable `div` fails `@angular-eslint/template` a11y rules (`click-events-have-key-events`, `interactive-supports-focus`) and would need manual keydown handling to match |
| Date picker models value as ISO `yyyy-MM-dd` string, not `Date` | JSON-serializable straight to/from an API without a conversion step at the form boundary; also matches what `<input type="date">` would have given a consuming component, so it's a drop-in replacement |

## Potential future work

- **Dark mode** — add `.dark` class toggle with persisted preference
- **Form validation UX** — inline field-level error messages with Angular Validators
- **Audit log** — track who changed branding settings and when
- **Performance budgets** — refine Angular budgets as feature count grows
- **E2E tests** — Playwright or Cypress for the auth + settings flow
- **Docker Compose with frontend** — add nginx reverse proxy for prod-like local stack
- **AutoMapper replacement** — current AutoMapper package has a known vulnerability; consider manual mapping or a lighter alternative
