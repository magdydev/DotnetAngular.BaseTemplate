import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';

export type HealthStatus = 'checking' | 'healthy' | 'unhealthy';

@Injectable({ providedIn: 'root' })
export class HealthService {
  private readonly http = inject(HttpClient);

  readonly status = signal<HealthStatus>('checking');
  readonly lastCheck = signal<Date | null>(null);

  check(): void {
    this.status.set('checking');
    this.http.get(`${environment.apiBaseUrl}/v1/health`, { responseType: 'json' })
      .pipe(catchError(() => of(null)))
      .subscribe({
        next: (res) => {
          this.status.set(res ? 'healthy' : 'unhealthy');
          this.lastCheck.set(new Date());
        },
        error: () => {
          this.status.set('unhealthy');
          this.lastCheck.set(new Date());
        },
      });
  }
}
