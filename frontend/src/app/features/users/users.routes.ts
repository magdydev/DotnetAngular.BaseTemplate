import { Routes } from '@angular/router';
import { adminGuard } from '../../core/auth/auth.guard';

export const USERS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [adminGuard],
    loadComponent: () => import('./user-management/user-management.component').then((m) => m.UserManagementComponent),
  },
];
