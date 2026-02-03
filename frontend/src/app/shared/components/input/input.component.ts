import { Component, input, output, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { cn } from '../../utils/class-name.util';

/**
 * Input size variants
 */
type InputSize = 'sm' | 'default' | 'lg';

/**
 * Reusable input component with validation support
 * @example
 * <exe-input
 *   type="email"
 *   label="Email"
 *   placeholder="user@example.com"
 *   [(ngModel)]="email"
 *   [required]="true"
 *   [error]="errors?.email"
 * />
 */
@Component({
  selector: 'exe-input',
  standalone: true,
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    }
  ]
})
export class InputComponent implements ControlValueAccessor {
  /** Input type */
  readonly type = input<'text' | 'email' | 'password' | 'number' | 'tel' | 'url'>('text');

  /** Input label */
  readonly label = input<string>('');

  /** Placeholder text */
  readonly placeholder = input<string>('');

  /** Required field indicator */
  readonly required = input<boolean>(false);

  /** Disabled state */
  readonly disabled = input<boolean>(false);

  /** Readonly state */
  readonly readonly = input<boolean>(false);

  /** Input size */
  readonly size = input<InputSize>('default');

  /** Helper text to display below input */
  readonly helperText = input<string>('');

  /** Error message to display */
  readonly error = input<string>('');

  /** Icon to display inside input (SVG or CSS class) */
  readonly icon = input<string>('');

  /** Icon position */
  readonly iconPosition = input<'left' | 'right'>('left');

  /** Full width input */
  readonly fullWidth = input<boolean>(true);

  /** Maximum length for text inputs */
  readonly maxLength = input<number>();

  /** Minimum value for number inputs */
  readonly min = input<number>();

  /** Maximum value for number inputs */
  readonly max = input<number>();

  /** Step value for number inputs */
  readonly step = input<number | 'any'>('any');

  /** Unique ID for the input */
  readonly id = input<string>(() => `input-${Math.random().toString(36).substr(2, 9)}`);

  /** Emit when input loses focus */
  readonly blur = output<void>();

  /** Internal value */
  private _value: string = '';

  /** onChange callback for ControlValueAccessor */
  private _onChange: (value: string) => void = () => {};

  /** onTouched callback for ControlValueAccessor */
  private _onTouched: () => void = () => {};

  /** Computed class name based on state */
  protected computedClass = computed(() => {
    return cn(
      'input',
      'block w-full rounded-md border border-charcoal-300 bg-white px-3 py-2',
      'text-sm placeholder:text-charcoal-400',
      'focus:border-insight-blue-500 focus:outline-none focus:ring-1 focus:ring-insight-blue-500',
      'disabled:bg-charcoal-100 disabled:cursor-not-allowed',
      {
        'border-red-500 focus:border-red-500 focus:ring-red-500': this.error(),
        'pl-10': this.icon() && this.iconPosition() === 'left',
        'pr-10': this.icon() && this.iconPosition() === 'right',
        'h-8 text-xs': this.size() === 'sm',
        'h-10': this.size() === 'default',
        'h-12 text-base': this.size() === 'lg'
      }
    );
  });

  /** Get current value */
  get value(): string {
    return this._value;
  }

  /** Set value from outside (ControlValueAccessor) */
  set value(value: string) {
    this._value = value;
    this._onChange(value);
  }

  /** Handle input changes */
  handleInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this._value = target.value;
    this._onChange(target.value);
  }

  /** Handle blur event */
  handleBlur(): void {
    this._onTouched();
    this.blur.emit();
  }

  /** ControlValueAccessor: Write value to component */
  writeValue(value: string): void {
    this._value = value || '';
  }

  /** ControlValueAccessor: Register onChange callback */
  registerOnChange(fn: (value: string) => void): void {
    this._onChange = fn;
  }

  /** ControlValueAccessor: Register onTouched callback */
  registerOnTouched(fn: () => void): void {
    this._onTouched = fn;
  }

  /** ControlValueAccessor: Set disabled state */
  setDisabledState(isDisabled: boolean): void {
    // Handle through input binding
  }

  /** Computed helper function (Angular doesn't have computed() yet, using getter) */
  protected computed(): any {
    return {
      computedClass: this.computedClass
    };
  }
}

// Simple computed function for Angular (Angular 19+ will have proper computed())
function computed(fn: () => string): () => string {
  return fn;
}
