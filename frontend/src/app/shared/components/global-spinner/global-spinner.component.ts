import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-global-spinner',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (loadingService.loading()) {
      <div class="spinner-backdrop">
        <div class="spinner"></div>
      </div>
    }
  `,
  styles: [`
    .spinner-backdrop {
      position: fixed;
      inset: 0;
      z-index: 9999;
      display: flex;
      align-items: center;
      justify-content: center;
      background: oklch(0% 0 0 / 20%);
      backdrop-filter: blur(2px);
    }

    .spinner {
      inline-size: 36px;
      block-size: 36px;
      border: 3px solid var(--color-border);
      border-top-color: var(--color-primary);
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `],
})
export class GlobalSpinnerComponent {
  protected readonly loadingService = inject(LoadingService);
}
