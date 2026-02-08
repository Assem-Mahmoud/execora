import { Component, Input, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-password-strength',
  standalone: true,
  template: `
    <div class="password-strength">
      <div class="strength-bar">
        <div class="strength-fill" [style.width]="getStrengthPercentage() + '%'" [style.backgroundColor]="strengthColor"></div>
      </div>
      <div class="strength-text" [style.color]="strengthColor">{{ strengthText }}</div>
    </div>
  `,
  styles: [`
    .password-strength {
      margin-top: 0.5rem;
    }

    .strength-bar {
      height: 4px;
      background-color: #e2e8f0;
      border-radius: 2px;
      overflow: hidden;
    }

    .strength-fill {
      height: 100%;
      transition: width 0.3s ease, background-color 0.3s ease;
    }

    .strength-text {
      font-size: 0.75rem;
      font-weight: 600;
      margin-top: 0.25rem;
    }
  `]
})
export class PasswordStrengthComponent {
  @Input() password: string = '';
  @Input() strengthColor: string = '#e2e8f0';
  @Input() strengthText: string = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['password'] && !changes['password'].firstChange) {
      this.updateStrength();
    }
  }

  ngOnInit(): void {
    this.updateStrength();
  }

  private updateStrength(): void {
    if (!this.password) {
      this.strengthColor = '#e2e8f0';
      this.strengthText = '';
      return;
    }

    const score = this.calculatePasswordScore(this.password);

    switch (score) {
      case 0:
        this.strengthColor = '#f5222d'; // red
        this.strengthText = 'Very Weak';
        break;
      case 1:
      case 2:
        this.strengthColor = '#fa8c16'; // orange
        this.strengthText = 'Weak';
        break;
      case 3:
        this.strengthColor = '#1890ff'; // blue
        this.strengthText = 'Medium';
        break;
      case 4:
        this.strengthColor = '#52c41a'; // green
        this.strengthText = 'Strong';
        break;
      case 5:
        this.strengthColor = '#52c41a'; // green
        this.strengthText = 'Very Strong';
        break;
    }
  }

  private calculatePasswordScore(password: string): number {
    let score = 0;

    // Length check
    if (password.length >= 12) score++;
    if (password.length >= 16) score++;

    // Character variety
    if (/[a-z]/.test(password)) score++; // lowercase
    if (/[A-Z]/.test(password)) score++; // uppercase
    if (/[0-9]/.test(password)) score++; // numbers
    if (/[^a-zA-Z0-9]/.test(password)) score++; // special characters

    return Math.min(score, 5);
  }

  private getStrengthPercentage(): number {
    const score = this.calculatePasswordScore(this.password);
    return (score / 5) * 100;
  }
}