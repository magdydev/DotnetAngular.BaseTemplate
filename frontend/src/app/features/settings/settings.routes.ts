import { Routes } from '@angular/router';
import { authGuard } from '../../core/auth/auth.guard';

export const SETTINGS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./branding-settings/branding-settings.component').then((m) => m.BrandingSettingsComponent),
  },
];
