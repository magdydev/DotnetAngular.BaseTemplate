import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { BrandingService } from '../../../core/services/branding.service';
import { LanguageService } from '../../../core/services/language.service';
import { ToastService } from '../../../core/services/toast.service';
import { logoSource } from '../../../core/models/branding.model';

@Component({
  selector: 'app-branding-settings',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './branding-settings.component.html',
  styleUrl: './branding-settings.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BrandingSettingsComponent {
  private readonly brandingService = inject(BrandingService);
  private readonly title = inject(Title);
  private readonly toast = inject(ToastService);
  private readonly languageService = inject(LanguageService);
  private readonly translate = inject(TranslateService);

  readonly saving = signal(false);
  readonly logoPreview = signal<string | null>(null);

  readonly appName = signal(this.brandingService.branding().appName);
  readonly appNameAr = signal(this.brandingService.branding().appNameAr);
  readonly primaryColor = signal(this.brandingService.branding().primaryColor);
  readonly secondaryColor = signal(this.brandingService.branding().secondaryColor);

  private readonly logoDataState = signal<string | null>(this.brandingService.branding().logoData);

  protected readonly logoSource = logoSource;

  private readonly titleAppName = computed(() => {
    const b = this.brandingService.branding();
    return this.languageService.currentLang() === 'ar' ? (b.appNameAr || b.appName) : b.appName;
  });

  constructor() {
    this.title.setTitle(this.titleAppName());
    this.syncLogoPreview();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      this.logoDataState.set(result);
      this.logoPreview.set(result);
    };
    reader.readAsDataURL(file);
  }

  removeLogo(): void {
    this.logoDataState.set(null);
    this.logoPreview.set(null);
  }

  async save(): Promise<void> {
    this.saving.set(true);
    try {
      const logoData = this.logoDataState();
      await this.brandingService.update({
        appName: this.appName(),
        appNameAr: this.appNameAr(),
        logoUrl: logoData ? null : this.brandingService.branding().logoUrl,
        logoData: logoData,
        primaryColor: this.primaryColor(),
        secondaryColor: this.secondaryColor(),
      });
      this.toast.success(this.translate.instant('TOAST.SETTINGS_SAVED'));
      this.title.setTitle(this.titleAppName());
    } catch {
      this.toast.error(this.translate.instant('TOAST.SETTINGS_SAVE_ERROR'));
    } finally {
      this.saving.set(false);
    }
  }

  private syncLogoPreview(): void {
    const src = logoSource(this.brandingService.branding());
    if (src) this.logoPreview.set(src);
  }
}
