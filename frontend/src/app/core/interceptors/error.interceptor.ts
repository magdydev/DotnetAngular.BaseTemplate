import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

/**
 * Centralizes HTTP error handling so feature services/components don't each
 * need their own 401/error boilerplate.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        router.navigate(['/login']);
      } else if (error.status === 0) {
        console.error('Network error — is the API reachable?', error);
      } else {
        console.error(`API error ${error.status} on ${req.method} ${req.url}`, error.error);
      }

      return throwError(() => error);
    }),
  );
};
