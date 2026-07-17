import { HttpInterceptorFn } from '@angular/common/http';

const TOKEN_STORAGE_KEY = 'access_token';

/**
 * Attaches the bearer token (once auth is implemented) to every outgoing
 * request. Functional interceptor — the modern (Angular 15+) replacement
 * for class-based HttpInterceptor, registered via provideHttpClient(withInterceptors([...])).
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);

  if (!token) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
