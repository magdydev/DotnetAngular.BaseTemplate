import { ChangeDetectionStrategy, Component, HostListener, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmService } from '../../../core/services/confirm.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [TranslatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (confirmService.state(); as state) {
      <div class="confirm-overlay">
        <button type="button" class="confirm-backdrop" aria-label="Close" (click)="cancel()"></button>
        <div class="confirm-card" role="alertdialog" aria-modal="true" [attr.aria-label]="state.title">
          <h2 class="confirm-title">{{ state.title }}</h2>
          <p class="confirm-message">{{ state.message }}</p>
          <div class="confirm-actions">
            <button type="button" class="confirm-btn confirm-btn--ghost" (click)="cancel()">
              {{ state.cancelText || ('CONFIRM.CANCEL' | translate) }}
            </button>
            <button
              type="button"
              class="confirm-btn"
              [class.confirm-btn--danger]="state.danger"
              [class.confirm-btn--primary]="!state.danger"
              (click)="confirm()"
            >
              {{ state.confirmText || ('CONFIRM.CONFIRM' | translate) }}
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .confirm-overlay {
      position: fixed;
      inset: 0;
      z-index: 10001;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--space-4);
      animation: confirm-fade 0.15s var(--ease-out);
    }

    .confirm-backdrop {
      position: absolute;
      inset: 0;
      border: none;
      padding: 0;
      background: oklch(0% 0 0 / 32%);
      backdrop-filter: blur(2px);
      cursor: pointer;
    }

    .confirm-card {
      position: relative;
      z-index: 1;
      background: var(--surface-card);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      padding: var(--space-6);
      max-width: 28rem;
      width: 100%;
      animation: confirm-pop 0.18s var(--ease-out);
    }

    .confirm-title {
      font-family: var(--font-heading);
      font-size: var(--text-lg);
      font-weight: var(--weight-bold);
      color: var(--color-text);
      margin: 0 0 var(--space-2);
    }

    .confirm-message {
      font-size: var(--text-sm);
      color: var(--color-text-muted);
      margin: 0 0 var(--space-6);
      line-height: 1.5;
    }

    .confirm-actions {
      display: flex;
      justify-content: flex-end;
      gap: var(--space-3);
    }

    .confirm-btn {
      font-family: var(--font-body);
      font-size: var(--text-sm);
      font-weight: var(--weight-semibold);
      padding: var(--space-2) var(--space-5);
      border: none;
      border-radius: var(--radius-md);
      cursor: pointer;
    }

    .confirm-btn--ghost {
      background: transparent;
      color: var(--color-text-muted);
      border: 1px solid var(--color-border);
    }

    .confirm-btn--ghost:hover {
      background: var(--color-bg-subtle);
      color: var(--color-text);
    }

    .confirm-btn--primary {
      background: linear-gradient(135deg, var(--color-primary), var(--color-primary-dark));
      color: var(--color-primary-contrast);
    }

    .confirm-btn--danger {
      background: var(--color-error);
      color: var(--color-error-contrast);
    }

    @keyframes confirm-fade {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    @keyframes confirm-pop {
      from { opacity: 0; transform: scale(0.96) translateY(4px); }
      to   { opacity: 1; transform: scale(1) translateY(0); }
    }
  `],
})
export class ConfirmDialogComponent {
  protected readonly confirmService = inject(ConfirmService);

  confirm(): void {
    this.confirmService.respond(true);
  }

  cancel(): void {
    this.confirmService.respond(false);
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.confirmService.state()) {
      this.cancel();
    }
  }
}
