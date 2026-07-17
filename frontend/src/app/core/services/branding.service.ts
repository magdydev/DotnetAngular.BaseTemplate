import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BrandingSettings } from '../models/branding.model';

const DEFAULT_BRANDING: BrandingSettings = {
  appName: 'BaseTemplate',
  logoUrl: 'assets/logo.svg',
  primaryColor: '#4F46E5',
  secondaryColor: '#F59E0B',
};

/**
 * Name/logo/colors are a database-backed setting (see BrandingController on
 * the API), not a build-time constant — so branding can change without a
 * frontend redeploy. Loaded once at startup via the APP_INITIALIZER in
 * app.config.ts and applied to the document; falls back to the defaults
 * above if the API is unreachable so the app never blocks on it.
 */
@Injectable({ providedIn: 'root' })
export class BrandingService {
  private readonly http = inject(HttpClient);
  private readonly titleService = inject(Title);

  private readonly _branding = signal<BrandingSettings>(DEFAULT_BRANDING);
  readonly branding = this._branding.asReadonly();

  async load(): Promise<void> {
    try {
      const settings = await firstValueFrom(
        this.http.get<BrandingSettings>(`${environment.apiBaseUrl}/v1/branding`),
      );
      this._branding.set(settings);
    } catch {
      // API unreachable or not seeded yet — keep the defaults above rather
      // than failing app startup.
    }
    this.applyToDocument();
  }

  private applyToDocument(): void {
    const branding = this._branding();
    this.titleService.setTitle(branding.appName);

    const root = document.documentElement.style;
    root.setProperty('--color-primary', branding.primaryColor);
    root.setProperty('--color-secondary', branding.secondaryColor);
  }
}
