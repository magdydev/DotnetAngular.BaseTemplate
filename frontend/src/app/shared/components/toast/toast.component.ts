import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast toast--{{ toast.type }}" role="alert">
          {{ toast.message }}
          <button type="button" class="toast-close" (click)="toastService.remove(toast.id)">&times;</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: var(--space-6);
      inset-inline-end: var(--space-6);
      z-index: 10000;
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      max-width: 36rem;
      min-width: 20rem;
    }

    .toast {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: var(--space-4);
      padding: var(--space-4) var(--space-5);
      border-radius: var(--radius-lg);
      font-size: var(--text-base);
      font-weight: var(--weight-medium);
      box-shadow: var(--shadow-lg), 0 0 0 1px oklch(0% 0 0 / 6%);
      animation: toast-in 0.3s var(--ease-out);
      line-height: 1.5;
    }

    .toast--success {
      background: var(--color-success);
      color: var(--color-success-contrast);
    }

    .toast--error {
      background: var(--color-error);
      color: var(--color-error-contrast);
    }

    .toast--info {
      background: var(--color-info);
      color: var(--color-info-contrast);
    }

    .toast-close {
      background: none;
      border: none;
      color: inherit;
      font-size: var(--text-lg);
      cursor: pointer;
      line-height: 1;
      opacity: 0.7;
    }

    .toast-close:hover {
      opacity: 1;
    }

    :dir(rtl) .toast {
      animation-name: toast-in-rtl;
    }

    @keyframes toast-in {
      from { opacity: 0; transform: translateX(100%); }
      to   { opacity: 1; transform: translateX(0); }
    }

    @keyframes toast-in-rtl {
      from { opacity: 0; transform: translateX(-100%); }
      to   { opacity: 1; transform: translateX(0); }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastComponent {
  protected readonly toastService = inject(ToastService);
}
