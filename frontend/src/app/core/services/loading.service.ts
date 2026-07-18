import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly requestCount = signal(0);
  readonly loading = signal(false);

  addRequest(): void {
    this.requestCount.update(c => c + 1);
    this.loading.set(true);
  }

  removeRequest(): void {
    this.requestCount.update(c => Math.max(0, c - 1));
    if (this.requestCount() === 0) {
      this.loading.set(false);
    }
  }
}
