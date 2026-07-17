export interface Product {
  id: string;
  name: string;
  sku: string;
  description?: string;
  price: number;
  currency: string;
  createdAt: string;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  price: number;
  currency: string;
  description?: string;
}
