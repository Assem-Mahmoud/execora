import { Component, input, output, computed, signal } from '@angular/core';
import { cva, type VariantProps } from 'class-variance-authority';
import { cn } from '../../utils/class-name.util';

/**
 * Button variants using class-variance-authority
 */
const buttonVariants = cva(
  'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        primary: 'bg-crimson-600 text-white hover:bg-crimson-700',
        secondary: 'bg-charcoal-700 text-white hover:bg-charcoal-800',
        outline: 'border border-charcoal-300 bg-transparent hover:bg-charcoal-100',
        ghost: 'hover:bg-charcoal-100',
        link: 'text-insight-blue-600 underline-offset-4 hover:underline',
        danger: 'bg-red-600 text-white hover:bg-red-700',
        success: 'bg-green-600 text-white hover:bg-green-700'
      },
      size: {
        sm: 'h-8 px-3 text-xs',
        default: 'h-10 px-4 py-2',
        lg: 'h-12 px-6 text-base'
      }
    },
    defaultVariants: {
      variant: 'primary',
      size: 'default'
    }
  }
);

/**
 * Button variant type
 */
export type ButtonVariant = VariantProps<typeof buttonVariants>['variant'];
export type ButtonSize = VariantProps<typeof buttonVariants>['size'];

/**
 * Reusable button component with multiple variants
 * @example
 * <exe-button variant="primary" size="lg" (click)="handleSubmit()">Submit</exe-button>
 * <exe-button variant="outline" [loading]="true">Loading...</exe-button>
 */
@Component({
  selector: 'exe-button',
  standalone: true,
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent {
  /** Button variant */
  readonly variant = input<ButtonVariant>('primary');

  /** Button size */
  readonly size = input<ButtonSize>('default');

  /** Disable the button */
  readonly disabled = input<boolean>(false);

  /** Show loading state (spinner) */
  readonly loading = input<boolean>(false);

  /** Icon to display (CSS class or SVG) */
  readonly icon = input<string>('');

  /** Icon position */
  readonly iconPosition = input<'left' | 'right'>('left');

  /** Button type */
  readonly type = input<'button' | 'submit' | 'reset'>('button');

  /** Full width button */
  readonly fullWidth = input<boolean>(false);

  /** Click event output */
  readonly click = output<MouseEvent>();

  /** Internal signal to track if button has been clicked (for double-click prevention) */
  protected clicked = signal(false);

  /** Computed class name based on variants */
  protected computedClass = computed(() => {
    return cn(
      buttonVariants({
        variant: this.loading() || this.clicked() ? 'primary' : this.variant(),
        size: this.size()
      }),
      this.fullWidth() ? 'w-full' : ''
    );
  });

  /**
   * Handle click event with loading state support
   */
  handleClick(event: MouseEvent): void {
    if (this.disabled() || this.loading() || this.clicked()) {
      event.preventDefault();
      return;
    }

    this.clicked.set(true);
    this.click.emit(event);

    // Reset clicked state after a short delay (prevents double-clicks)
    setTimeout(() => {
      this.clicked.set(false);
    }, 300);
  }

  /**
   * Check if icon should be shown on the left
   */
  showIconLeft(): boolean {
    return this.iconPosition() === 'left' && !!this.icon();
  }

  /**
   * Check if icon should be shown on the right
   */
  showIconRight(): boolean {
    return this.iconPosition() === 'right' && !!this.icon();
  }
}
