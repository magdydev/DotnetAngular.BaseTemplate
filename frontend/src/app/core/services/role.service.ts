import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RoleDto } from '../models/role.model';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/roles`;

  private readonly _roles = signal<RoleDto[]>([]);
  readonly roles = this._roles.asReadonly();

  async loadAll(): Promise<void> {
    const roles = await firstValueFrom(this.http.get<RoleDto[]>(this.baseUrl));
    this._roles.set(roles);
  }

  async create(name: string): Promise<void> {
    const role = await firstValueFrom(this.http.post<RoleDto>(this.baseUrl, { name }));
    this._roles.update((list) => [...list, role].sort((a, b) => a.name.localeCompare(b.name)));
  }

  async remove(id: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${id}`));
    this._roles.update((list) => list.filter((r) => r.id !== id));
  }
}
