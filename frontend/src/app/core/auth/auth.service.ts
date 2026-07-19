import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../../../environments/environment';
import { ToastService } from '../services/toast.service';

const TOKEN_KEY = 'access_token';
const ROLES_KEY = 'user_roles';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  appName: string;
  roles: string[];
}

function readStoredRoles(): string[] {
  try {
    const raw = localStorage.getItem(ROLES_KEY);
    return raw ? (JSON.parse(raw) as string[]) : [];
  } catch {
    return [];
  }
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  private readonly _isAuthenticated = signal(!!localStorage.getItem(TOKEN_KEY));
  readonly isAuthenticated = this._isAuthenticated.asReadonly();

  private readonly _roles = signal<string[]>(readStoredRoles());
  readonly roles = this._roles.asReadonly();
  readonly isAdmin = computed(() => this._roles().includes('Admin'));

  async login(request: LoginRequest): Promise<void> {
    const res = await firstValueFrom(
      this.http.post<LoginResponse>(`${environment.apiBaseUrl}/v1/auth/login`, request),
    );
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(ROLES_KEY, JSON.stringify(res.roles));
    this._isAuthenticated.set(true);
    this._roles.set(res.roles);
    this.toast.success(this.translate.instant('TOAST.SIGNED_IN'));
    this.router.navigate(['/settings']);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLES_KEY);
    this._isAuthenticated.set(false);
    this._roles.set([]);
    this.toast.info(this.translate.instant('TOAST.SIGNED_OUT'));
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }
}
