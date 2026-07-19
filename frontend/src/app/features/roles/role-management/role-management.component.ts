import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RoleService } from '../../../core/services/role.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';
import { RoleDto } from '../../../core/models/role.model';

@Component({
  selector: 'app-role-management',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './role-management.component.html',
  styleUrl: './role-management.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleManagementComponent {
  private readonly roleService = inject(RoleService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);
  private readonly confirmService = inject(ConfirmService);

  readonly roles = this.roleService.roles;
  readonly loading = signal(true);
  readonly creating = signal(false);
  readonly newRoleName = signal('');

  constructor() {
    this.load();
  }

  private async load(): Promise<void> {
    this.loading.set(true);
    try {
      await this.roleService.loadAll();
    } catch {
      this.toast.error(this.translate.instant('TOAST.ACTION_ERROR'));
    } finally {
      this.loading.set(false);
    }
  }

  async createRole(): Promise<void> {
    const name = this.newRoleName().trim();
    if (!name) return;

    this.creating.set(true);
    try {
      await this.roleService.create(name);
      this.toast.success(this.translate.instant('TOAST.ROLE_CREATED'));
      this.newRoleName.set('');
    } catch (err) {
      this.toast.error(this.extractError(err));
    } finally {
      this.creating.set(false);
    }
  }

  async deleteRole(role: RoleDto): Promise<void> {
    const confirmed = await this.confirmService.confirm({
      title: this.translate.instant('ROLES.DELETE'),
      message: this.translate.instant('ROLES.CONFIRM_DELETE', { name: role.name }),
      confirmText: this.translate.instant('ROLES.DELETE'),
      danger: true,
    });
    if (!confirmed) return;

    try {
      await this.roleService.remove(role.id);
      this.toast.success(this.translate.instant('TOAST.ROLE_DELETED'));
    } catch (err) {
      this.toast.error(this.extractError(err));
    }
  }

  private extractError(err: unknown): string {
    if (err && typeof err === 'object' && 'error' in err) {
      const body = (err as { error?: { message?: string } }).error;
      if (body?.message) return body.message;
    }
    return this.translate.instant('TOAST.ACTION_ERROR');
  }
}
