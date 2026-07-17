import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateProductRequest, Product } from '../models/product.model';

/**
 * Thin HTTP wrapper around the Products API. Components consume the
 * returned Observables directly via the async pipe rather than subscribing
 * manually — see ProductListComponent for the pattern.
 */
@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/products`;

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<string> {
    return this.http.post<string>(this.baseUrl, request);
  }
}
