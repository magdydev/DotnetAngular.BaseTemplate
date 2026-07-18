import { Routes } from '@angular/router';

/**
 * Top-level route table. Each feature is lazy-loaded via loadChildren so its
 * code only ships when a user actually navigates there. Add new features the
 * same way — a routes file per feature under src/app/features.
 */
export const routes: Routes = [
  { path: '', redirectTo: 'settings', pathMatch: 'full' },
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'settings',
    loadChildren: () => import('./features/settings/settings.routes').then((m) => m.SETTINGS_ROUTES),
  },
  { path: '**', redirectTo: 'settings' },
];
