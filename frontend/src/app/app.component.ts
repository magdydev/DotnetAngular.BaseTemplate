import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LanguageService } from './core/services/language.service';
import { HealthService } from './core/services/health.service';
import { AppLogService } from './core/services/app-log.service';
import { AppFooterComponent } from './shared/components/app-footer/app-footer.component';
import { AppHeaderComponent } from './shared/components/app-header/app-header.component';
import { AppSidebarComponent } from './shared/components/app-sidebar/app-sidebar.component';
import { GlobalSpinnerComponent } from './shared/components/global-spinner/global-spinner.component';
import { ToastComponent } from './shared/components/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AppHeaderComponent, AppFooterComponent, AppSidebarComponent, GlobalSpinnerComponent, ToastComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  private readonly languageService = inject(LanguageService);
  private readonly healthService = inject(HealthService);
  private readonly log = inject(AppLogService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly isAuthPage = signal(false);

  constructor() {
    this.languageService.init();
    this.log.info('App initialized');
    this.healthService.check();
    this.isAuthPage.set(this.router.url === '/login');
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(e => this.isAuthPage.set(e.url === '/login'));
  }
}
