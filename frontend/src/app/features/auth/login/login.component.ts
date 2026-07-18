import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { BrandingService } from '../../../core/services/branding.service';
import { LanguageService } from '../../../core/services/language.service';
import { ToastService } from '../../../core/services/toast.service';
import { logoSource } from '../../../core/models/branding.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly title = inject(Title);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);
  protected readonly brandingService = inject(BrandingService);
  protected readonly languageService = inject(LanguageService);
  protected readonly logoSrc = computed(() => logoSource(this.brandingService.branding()));
  protected readonly appName = computed(() => {
    const b = this.brandingService.branding();
    return this.languageService.currentLang() === 'ar' ? (b.appNameAr || b.appName) : b.appName;
  });

  toggleLanguage(): void {
    this.languageService.use(this.languageService.currentLang() === 'en' ? 'ar' : 'en');
  }

  readonly username = signal('');
  readonly password = signal('');
  readonly loading = signal(false);

  constructor() {
    this.title.setTitle(this.appName());
  }

  async onSubmit(): Promise<void> {
    this.loading.set(true);
    try {
      await this.authService.login({
        username: this.username(),
        password: this.password(),
      });
    } catch {
      this.toast.error(this.translate.instant('TOAST.INVALID_CREDENTIALS'));
    } finally {
      this.loading.set(false);
    }
  }
}
