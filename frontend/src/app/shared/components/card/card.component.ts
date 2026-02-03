import { Component, input, contentChild, TemplateRef } from '@angular/core';
import { cn } from '../../utils/class-name.util';

/**
 * Card variant types
 */
type CardVariant = 'default' | 'bordered' | 'elevated' | 'outlined';

/**
 * Reusable card component for grouping related content
 * @example
 * <exe-card variant="elevated" [padding]="true">
 *   <h3 card-title>Card Title</h3>
 *   <p card-content>Card content goes here.</p>
 *   <div card-actions>
 *     <exe-button>Action</exe-button>
 *   </div>
 * </exe-card>
 */
@Component({
  selector: 'exe-card',
  standalone: true,
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})
export class CardComponent {
  /** Card variant */
  readonly variant = input<CardVariant>('default');

  /** Add padding inside card */
  readonly padding = input<boolean>(true);

  /** Make card full width */
  readonly fullWidth = input<boolean>(false);

  /** Make card clickable (adds hover effect) */
  readonly clickable = input<boolean>(false);

  /** Show hover elevation effect */
  readonly hoverable = input<boolean>(false);

  /** Custom CSS classes */
  readonly customClass = input<string>('');

  /** Projected title content */
  readonly title = contentChild('card-title', { read: TemplateRef });

  /** Projected subtitle content */
  readonly subtitle = contentChild('card-subtitle', { read: TemplateRef });

  /** Projected content */
  readonly content = contentChild('card-content', { read: TemplateRef });

  /** Projected actions */
  readonly actions = contentChild('card-actions', { read: TemplateRef });

  /** Computed container class */
  protected containerClass = this.buildContainerClass();

  private buildContainerClass(): string {
    return cn(
      'card',
      {
        'card-bordered': this.variant() === 'bordered' || this.variant() === 'outlined',
        'card-elevated': this.variant() === 'elevated',
        'card-clickable': this.clickable() || this.hoverable(),
        'card-hoverable': this.hoverable(),
        'card-padding': this.padding(),
        'w-full': this.fullWidth()
      },
      this.customClass()
    );
  }
}
