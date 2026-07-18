# Plans

## Current features

- **Auth**: ASP.NET Core Identity with JWT, login page, auth guard (lazy-loaded)
- **Branding**: dynamic app name/logo/colors, stored in DB, editable via settings page (lazy-loaded, auth-guarded)
- **i18n**: English + Arabic with automatic RTL, translated toast messages
- **Design system**: OKLCH tokens, responsive layout (640/768/1024 breakpoints), accessible, dark-mode-ready
- **Toast notifications**: direction-aware (top-right LTR / top-left RTL), auto-dismiss, success/error/info
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

## Potential future work

- **User management page** — list/create/edit/delete users via Identity
- **Role management** — role CRUD since Admin role is already seeded
- **Dark mode** — add `.dark` class toggle with persisted preference
- **Confirmation dialogs** — reusable modal for destructive actions (delete, logout)
- **Form validation UX** — inline field-level error messages with Angular Validators
- **Audit log** — track who changed branding settings and when
- **Performance budgets** — refine Angular budgets as feature count grows
- **E2E tests** — Playwright or Cypress for the auth + settings flow
- **Docker Compose with frontend** — add nginx reverse proxy for prod-like local stack
- **AutoMapper replacement** — current AutoMapper package has a known vulnerability; consider manual mapping or a lighter alternative
