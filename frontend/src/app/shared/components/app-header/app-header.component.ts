import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { LanguageService } from '../../../core/services/language.service';
import { BrandingService } from '../../../core/services/branding.service';
import { logoSource } from '../../../core/models/branding.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppHeaderComponent {
  protected readonly languageService = inject(LanguageService);
  protected readonly brandingService = inject(BrandingService);
  protected readonly authService = inject(AuthService);

  protected readonly logoSrc = computed(() => logoSource(this.brandingService.branding()));

  toggleLanguage(): void {
    this.languageService.use(this.languageService.currentLang() === 'en' ? 'ar' : 'en');
  }

  logout(): void {
    this.authService.logout();
  }
}
