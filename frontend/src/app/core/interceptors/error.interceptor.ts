import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AppLogService } from '../services/app-log.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const log = inject(AppLogService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const ctx = {
        url: req.url,
        method: req.method,
        status: error.status,
      };

      if (error.status === 401) {
        log.warn('Unauthorized request, redirecting to login', ctx);
        router.navigate(['/login']);
      } else if (error.status === 0) {
        log.error('Network error — API unreachable', ctx);
      } else {
        log.error(`API error ${error.status}`, { ...ctx, body: error.error });
      }

      return throwError(() => error);
    }),
  );
};
