import { Routes } from '@angular/router';
import { adminGuard } from '../../core/auth/auth.guard';

export const ROLES_ROUTES: Routes = [
  {
    path: '',
    canActivate: [adminGuard],
    loadComponent: () => import('./role-management/role-management.component').then((m) => m.RoleManagementComponent),
  },
];
