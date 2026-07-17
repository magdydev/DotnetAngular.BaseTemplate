import { Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AppLanguage, LanguageService } from '../../../core/services/language.service';
import { BrandingService } from '../../../core/services/branding.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
})
export class AppHeaderComponent {
  protected readonly languageService = inject(LanguageService);
  protected readonly brandingService = inject(BrandingService);

  switchTo(lang: AppLanguage): void {
    this.languageService.use(lang);
  }
}
