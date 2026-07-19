import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppUserDto, CreateUserRequest, UpdateUserRequest } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/users`;

  private readonly _users = signal<AppUserDto[]>([]);
  readonly users = this._users.asReadonly();

  async loadAll(): Promise<void> {
    const users = await firstValueFrom(this.http.get<AppUserDto[]>(this.baseUrl));
    this._users.set(users);
  }

  async create(request: CreateUserRequest): Promise<void> {
    const user = await firstValueFrom(this.http.post<AppUserDto>(this.baseUrl, request));
    this._users.update((list) => [...list, user].sort((a, b) => a.username.localeCompare(b.username)));
  }

  async update(id: string, request: UpdateUserRequest): Promise<void> {
    const updated = await firstValueFrom(this.http.put<AppUserDto>(`${this.baseUrl}/${id}`, request));
    this._users.update((list) => list.map((u) => (u.id === id ? updated : u)));
  }

  async resetPassword(id: string, newPassword: string): Promise<void> {
    await firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}/password`, { newPassword }));
  }

  async remove(id: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${id}`));
    this._users.update((list) => list.filter((u) => u.id !== id));
  }
}
