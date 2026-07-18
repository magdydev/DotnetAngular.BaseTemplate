import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info';
}

let nextId = 0;

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<Toast[]>([]);

  success(message: string): void {
    this.add(message, 'success');
  }

  error(message: string): void {
    this.add(message, 'error');
  }

  info(message: string): void {
    this.add(message, 'info');
  }

  remove(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private add(message: string, type: Toast['type']): void {
    const toast: Toast = { id: ++nextId, message, type };
    this.toasts.update(list => [...list, toast]);
    setTimeout(() => this.remove(toast.id), 4000);
  }
}
