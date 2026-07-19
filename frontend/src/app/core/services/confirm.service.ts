import { Injectable, signal } from '@angular/core';

export interface ConfirmOptions {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  /** Styles the confirm button as destructive (red) instead of primary. */
  danger?: boolean;
}

interface ConfirmState extends ConfirmOptions {
  resolve: (result: boolean) => void;
}

/**
 * Promise-based replacement for the browser's native confirm() — same call
 * shape (`await confirmService.confirm(...)` returns a boolean), but renders
 * as an in-app modal (see ConfirmDialogComponent) that matches the app's
 * design system and translates.
 */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  readonly state = signal<ConfirmState | null>(null);

  confirm(options: ConfirmOptions): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
      this.state.set({ ...options, resolve });
    });
  }

  respond(result: boolean): void {
    this.state()?.resolve(result);
    this.state.set(null);
  }
}
