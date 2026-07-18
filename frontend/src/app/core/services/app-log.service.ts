import { Injectable } from '@angular/core';

export type LogLevel = 'info' | 'warn' | 'error';

export interface LogEntry {
  timestamp: string;
  level: LogLevel;
  message: string;
  context?: Record<string, unknown>;
}

@Injectable({ providedIn: 'root' })
export class AppLogService {
  private readonly logs: LogEntry[] = [];
  private readonly maxLogs = 200;

  info(message: string, context?: Record<string, unknown>): void {
    this.log({ level: 'info', message, context });
  }

  warn(message: string, context?: Record<string, unknown>): void {
    this.log({ level: 'warn', message, context });
  }

  error(message: string, context?: Record<string, unknown>): void {
    this.log({ level: 'error', message, context });
  }

  getAll(): LogEntry[] {
    return this.logs;
  }

  private log(entry: Omit<LogEntry, 'timestamp'>): void {
    const log: LogEntry = {
      ...entry,
      timestamp: new Date().toISOString(),
    };

    this.logs.unshift(log);
    if (this.logs.length > this.maxLogs) {
      this.logs.length = this.maxLogs;
    }

    switch (log.level) {
      case 'error':
        console.error(`[${log.timestamp}] ${log.message}`, log.context ?? '');
        break;
      case 'warn':
        console.warn(`[${log.timestamp}] ${log.message}`, log.context ?? '');
        break;
      default:
        console.log(`[${log.timestamp}] ${log.message}`, log.context ?? '');
    }
  }
}
