import { Routes } from '@angular/router';

/**
 * Top-level route table. Each feature is lazy-loaded via loadChildren so its
 * code only ships when a user actually navigates there. Add new features the
 * same way — a routes file per feature under src/app/features.
 */
export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'products',
    loadChildren: () => import('./features/products/products.routes').then((m) => m.PRODUCTS_ROUTES),
  },
  { path: '**', redirectTo: 'products' },
];
