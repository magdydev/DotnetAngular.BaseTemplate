import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  return router.parseUrl('/login');
};

/**
 * Client-side gate only — hides admin-only routes from non-admins for UX.
 * The real enforcement is server-side ([Authorize(Roles = "Admin")] on
 * UsersController/RolesController); this just avoids a confusing 403.
 */
export const adminGuard = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    return router.parseUrl('/login');
  }

  if (!auth.isAdmin()) {
    return router.parseUrl('/settings');
  }

  return true;
};
