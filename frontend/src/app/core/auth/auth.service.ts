import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../../../environments/environment';
import { ToastService } from '../services/toast.service';

const TOKEN_KEY = 'access_token';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  appName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  private readonly _isAuthenticated = signal(!!localStorage.getItem(TOKEN_KEY));
  readonly isAuthenticated = this._isAuthenticated.asReadonly();

  async login(request: LoginRequest): Promise<void> {
    const res = await firstValueFrom(
      this.http.post<LoginResponse>(`${environment.apiBaseUrl}/v1/auth/login`, request),
    );
    localStorage.setItem(TOKEN_KEY, res.token);
    this._isAuthenticated.set(true);
    this.toast.success(this.translate.instant('TOAST.SIGNED_IN'));
    this.router.navigate(['/settings']);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this._isAuthenticated.set(false);
    this.toast.info(this.translate.instant('TOAST.SIGNED_OUT'));
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}
