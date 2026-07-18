import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

/**
 * Fixed agency attribution. Unlike the rest of the app's branding, this is
 * intentionally NOT touched by the template rename script — it stays
 * "Powered by MagdyTech Solutions" no matter what project this template
 * becomes.
 */
@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './app-footer.component.html',
  styleUrl: './app-footer.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppFooterComponent {}
