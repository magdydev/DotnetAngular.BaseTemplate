import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { UserService } from '../../../core/services/user.service';
import { RoleService } from '../../../core/services/role.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';
import { AppUserDto } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserManagementComponent {
  private readonly userService = inject(UserService);
  private readonly roleService = inject(RoleService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);
  private readonly confirmService = inject(ConfirmService);

  readonly users = this.userService.users;
  readonly roles = this.roleService.roles;
  readonly loading = signal(true);

  // Add-user form state.
  readonly creating = signal(false);
  readonly newUsername = signal('');
  readonly newEmail = signal('');
  readonly newPassword = signal('');
  readonly newRoles = signal<string[]>([]);

  // Inline edit state (one row at a time).
  readonly editingId = signal<string | null>(null);
  readonly editEmail = signal('');
  readonly editRoles = signal<string[]>([]);
  readonly savingEdit = signal(false);

  // Inline password-reset state (one row at a time).
  readonly resettingId = signal<string | null>(null);
  readonly resetPasswordValue = signal('');
  readonly savingReset = signal(false);

  constructor() {
    this.load();
  }

  private async load(): Promise<void> {
    this.loading.set(true);
    try {
      await Promise.all([this.userService.loadAll(), this.roleService.loadAll()]);
    } catch {
      this.toast.error(this.translate.instant('TOAST.ACTION_ERROR'));
    } finally {
      this.loading.set(false);
    }
  }

  toggleNewRole(role: string): void {
    this.newRoles.update((list) => (list.includes(role) ? list.filter((r) => r !== role) : [...list, role]));
  }

  toggleEditRole(role: string): void {
    this.editRoles.update((list) => (list.includes(role) ? list.filter((r) => r !== role) : [...list, role]));
  }

  async createUser(): Promise<void> {
    const username = this.newUsername().trim();
    if (!username || !this.newPassword()) return;

    this.creating.set(true);
    try {
      await this.userService.create({
        username,
        email: this.newEmail().trim() || null,
        password: this.newPassword(),
        roles: this.newRoles(),
      });
      this.toast.success(this.translate.instant('TOAST.USER_CREATED'));
      this.newUsername.set('');
      this.newEmail.set('');
      this.newPassword.set('');
      this.newRoles.set([]);
    } catch (err) {
      this.toast.error(this.extractError(err));
    } finally {
      this.creating.set(false);
    }
  }

  startEdit(user: AppUserDto): void {
    this.resettingId.set(null);
    this.editingId.set(user.id);
    this.editEmail.set(user.email ?? '');
    this.editRoles.set([...user.roles]);
  }

  cancelEdit(): void {
    this.editingId.set(null);
  }

  async saveEdit(id: string): Promise<void> {
    this.savingEdit.set(true);
    try {
      await this.userService.update(id, { email: this.editEmail().trim() || null, roles: this.editRoles() });
      this.toast.success(this.translate.instant('TOAST.USER_UPDATED'));
      this.editingId.set(null);
    } catch (err) {
      this.toast.error(this.extractError(err));
    } finally {
      this.savingEdit.set(false);
    }
  }

  startResetPassword(id: string): void {
    this.editingId.set(null);
    this.resettingId.set(id);
    this.resetPasswordValue.set('');
  }

  cancelResetPassword(): void {
    this.resettingId.set(null);
  }

  async confirmResetPassword(id: string): Promise<void> {
    if (!this.resetPasswordValue()) return;

    this.savingReset.set(true);
    try {
      await this.userService.resetPassword(id, this.resetPasswordValue());
      this.toast.success(this.translate.instant('TOAST.PASSWORD_RESET'));
      this.resettingId.set(null);
    } catch (err) {
      this.toast.error(this.extractError(err));
    } finally {
      this.savingReset.set(false);
    }
  }

  async deleteUser(user: AppUserDto): Promise<void> {
    const confirmed = await this.confirmService.confirm({
      title: this.translate.instant('USERS.DELETE'),
      message: this.translate.instant('USERS.CONFIRM_DELETE', { username: user.username }),
      confirmText: this.translate.instant('USERS.DELETE'),
      danger: true,
    });
    if (!confirmed) return;

    try {
      await this.userService.remove(user.id);
      this.toast.success(this.translate.instant('TOAST.USER_DELETED'));
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
