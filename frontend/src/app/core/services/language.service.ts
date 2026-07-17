import { Injectable, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type AppLanguage = 'en' | 'ar';

const STORAGE_KEY = 'app_language';
const SUPPORTED_LANGUAGES: AppLanguage[] = ['en', 'ar'];
const RTL_LANGUAGES: AppLanguage[] = ['ar'];

/**
 * Owns language selection: persists the choice, drives ngx-translate, and
 * flips the document's `dir`/`lang` attributes so RTL layout (Arabic) just
 * works via CSS logical properties without a page reload.
 */
@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly translate = inject(TranslateService);

  /** Reactive current language — read as `languageService.currentLang()` in templates. */
  readonly currentLang = this.translate.currentLang;

  init(): void {
    this.translate.addLangs(SUPPORTED_LANGUAGES);

    const saved = localStorage.getItem(STORAGE_KEY) as AppLanguage | null;
    const browserLang = this.translate.getBrowserLang() as AppLanguage | undefined;
    const initial: AppLanguage =
      saved ?? (browserLang && SUPPORTED_LANGUAGES.includes(browserLang) ? browserLang : 'en');

    this.use(initial);
  }

  use(lang: AppLanguage): void {
    this.translate.use(lang);
    localStorage.setItem(STORAGE_KEY, lang);
    document.documentElement.lang = lang;
    document.documentElement.dir = RTL_LANGUAGES.includes(lang) ? 'rtl' : 'ltr';
  }
}
