import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { Product } from '../../../core/models/product.model';
import { ProductService } from '../../../core/services/product.service';

/**
 * Sample feature component demonstrating the pattern: a standalone
 * component that exposes an Observable straight to the template via the
 * async pipe, no manual subscribe/unsubscribe.
 */
@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [AsyncPipe, CurrencyPipe, TranslatePipe],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent {
  private readonly productService = inject(ProductService);

  readonly products$: Observable<Product[]> = this.productService.getAll();
}
