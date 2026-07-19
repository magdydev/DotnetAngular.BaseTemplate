import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  HostListener,
  computed,
  forwardRef,
  inject,
  input,
  signal,
} from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '../../../core/services/language.service';

interface CalendarCell {
  date: Date;
  day: number;
  iso: string;
  inCurrentMonth: boolean;
  isToday: boolean;
  selected: boolean;
  disabled: boolean;
}

function toIsoDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function parseIsoDate(value: string): Date | null {
  const parts = value.split('-').map(Number);
  const [year, month, day] = parts;
  if (!year || !month || !day) return null;
  return new Date(year, month - 1, day);
}

/**
 * Custom calendar date input — not a native <input type="date">. Drop it
 * into any form via [(ngModel)] or a reactive FormControl; the underlying
 * value is an ISO "yyyy-MM-dd" string (or null), so it's a JSON-serializable
 * drop-in wherever a date field is needed.
 */
@Component({
  selector: 'app-date-picker',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './date-picker.component.html',
  styleUrl: './date-picker.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true,
    },
  ],
})
export class DatePickerComponent implements ControlValueAccessor {
  private readonly elementRef = inject(ElementRef<HTMLElement>);
  private readonly languageService = inject(LanguageService);

  readonly placeholder = input('');
  readonly minDate = input<string | null>(null);
  readonly maxDate = input<string | null>(null);

  readonly open = signal(false);
  readonly disabled = signal(false);
  private readonly selected = signal<Date | null>(null);

  private readonly today = new Date();
  readonly viewYear = signal(this.today.getFullYear());
  readonly viewMonth = signal(this.today.getMonth());

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  private readonly locale = computed(() => (this.languageService.currentLang() === 'ar' ? 'ar' : 'en-US'));

  readonly displayValue = computed(() => {
    const date = this.selected();
    if (!date) return '';
    return new Intl.DateTimeFormat(this.locale(), { year: 'numeric', month: 'short', day: 'numeric' }).format(date);
  });

  readonly monthLabel = computed(() =>
    new Intl.DateTimeFormat(this.locale(), { month: 'long', year: 'numeric' }).format(
      new Date(this.viewYear(), this.viewMonth(), 1),
    ),
  );

  readonly weekdayLabels = computed(() => {
    const formatter = new Intl.DateTimeFormat(this.locale(), { weekday: 'narrow' });
    // 1970-01-04 was a Sunday — a fixed, arbitrary week to read labels off of.
    return Array.from({ length: 7 }, (_, i) => formatter.format(new Date(1970, 0, 4 + i)));
  });

  readonly calendarCells = computed<CalendarCell[]>(() => {
    const year = this.viewYear();
    const month = this.viewMonth();
    const firstOfMonth = new Date(year, month, 1);
    const start = new Date(year, month, 1 - firstOfMonth.getDay());

    const todayIso = toIsoDate(this.today);
    const selectedIso = this.selected() ? toIsoDate(this.selected()!) : null;
    const min = this.minDate() ? parseIsoDate(this.minDate()!) : null;
    const max = this.maxDate() ? parseIsoDate(this.maxDate()!) : null;

    return Array.from({ length: 42 }, (_, i) => {
      const date = new Date(start.getFullYear(), start.getMonth(), start.getDate() + i);
      const iso = toIsoDate(date);
      return {
        date,
        day: date.getDate(),
        iso,
        inCurrentMonth: date.getMonth() === month,
        isToday: iso === todayIso,
        selected: iso === selectedIso,
        disabled: (min !== null && date < min) || (max !== null && date > max),
      };
    });
  });

  writeValue(value: string | null): void {
    const date = value ? parseIsoDate(value) : null;
    this.selected.set(date);
    if (date) {
      this.viewYear.set(date.getFullYear());
      this.viewMonth.set(date.getMonth());
    }
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  toggleOpen(): void {
    if (this.disabled()) return;
    this.open.update((value) => !value);
  }

  prevMonth(): void {
    const month = this.viewMonth();
    if (month === 0) {
      this.viewMonth.set(11);
      this.viewYear.update((year) => year - 1);
    } else {
      this.viewMonth.set(month - 1);
    }
  }

  nextMonth(): void {
    const month = this.viewMonth();
    if (month === 11) {
      this.viewMonth.set(0);
      this.viewYear.update((year) => year + 1);
    } else {
      this.viewMonth.set(month + 1);
    }
  }

  selectDay(cell: CalendarCell): void {
    if (cell.disabled) return;
    this.commit(cell.date);
  }

  selectToday(): void {
    this.viewYear.set(this.today.getFullYear());
    this.viewMonth.set(this.today.getMonth());
    this.commit(this.today);
  }

  clear(): void {
    this.selected.set(null);
    this.onChange(null);
    this.onTouched();
    this.open.set(false);
  }

  private commit(date: Date): void {
    this.selected.set(date);
    this.onChange(toIsoDate(date));
    this.onTouched();
    this.open.set(false);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.open() && !this.elementRef.nativeElement.contains(event.target as Node)) {
      this.open.set(false);
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.open.set(false);
  }
}
