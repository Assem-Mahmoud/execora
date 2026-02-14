import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PasswordStrengthComponent } from '../../../shared/components/password-strength/password-strength.component';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest, RegisterResponse } from '../../../core/models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    PasswordStrengthComponent,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {
  registerForm: FormGroup;
  loading = false;
  submitted = false;

  // Password requirements
  passwordRequirements = {
    length: false,
    uppercase: false,
    lowercase: false,
    number: false,
    special: false
  };

  private subscription: Subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.registerForm = this.fb.group({
      email: ['', [
        Validators.required,
        Validators.email,
        Validators.maxLength(255)
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(12),
        Validators.maxLength(100)
      ]],
      firstName: ['', [
        Validators.required,
        Validators.maxLength(100),
        Validators.pattern("^[a-zA-Z '-]+$")
      ]],
      lastName: ['', [
        Validators.required,
        Validators.maxLength(100),
        Validators.pattern("^[a-zA-Z '-]+$")
      ]],
      organizationName: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100),
        Validators.pattern("^[a-zA-Z0-9 '-]+$")
      ]]
    });
  }

  ngOnInit(): void {
    // Check if user is already logged in
    if (localStorage.getItem('execora_access_token')) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  // Getter for easy access to form controls
  get f(): { [key: string]: AbstractControl } {
    return this.registerForm.controls;
  }

  // Handle password input changes
  onPasswordChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const password = input.value;
    this.passwordRequirements = {
      length: password.length >= 12,
      uppercase: /[A-Z]/.test(password),
      lowercase: /[a-z]/.test(password),
      number: /[0-9]/.test(password),
      special: /[^a-zA-Z0-9]/.test(password)
    };

    // Update password validation rules
    const passwordControl = this.f['password'];
    passwordControl.updateValueAndValidity();
  }

  // Submit registration form
  onSubmit(): void {
    this.submitted = true;

    // Stop here if form is invalid
    if (this.registerForm.invalid) {
      return;
    }

    // Check password requirements
    const allRequirementsMet = Object.values(this.passwordRequirements).every(req => req);
    if (!allRequirementsMet) {
      this.snackBar.open('Password does not meet all requirements', 'Close', { duration: 5000 });
      return;
    }

    this.loading = true;

    // Call API to register user
    const registerData: RegisterRequest = this.registerForm.value;

    const subscription = this.authService.register(registerData).subscribe({
      next: (response: RegisterResponse) => {
        this.snackBar.open('Registration successful! Please check your email for verification.', 'Close', { duration: 5000 });

        // Redirect to verify-email page with email (token is sent via email only for security)
        this.router.navigate(['/verify-email'], {
          queryParams: {
            email: registerData.email
          }
        });
      },
      error: (error: any) => {
        this.loading = false;
        this.snackBar.open(error?.error?.detail || 'Registration failed. Please try again.', 'Close', { duration: 5000 });
        console.error('Registration error:', error);
      }
    });

    this.subscription.add(subscription);
  }

  // Get password strength color
  getPasswordStrengthColor(): string {
    const metCount = Object.values(this.passwordRequirements).filter(req => req).length;
    if (metCount === 0) return '#f5222d'; // red
    if (metCount <= 2) return '#fa8c16'; // orange
    if (metCount <= 3) return '#1890ff'; // blue
    if (metCount <= 4) return '#52c41a'; // green
    return '#52c41a'; // green for perfect
  }

  // Get password strength text
  getPasswordStrengthText(): string {
    const metCount = Object.values(this.passwordRequirements).filter(req => req).length;
    if (metCount === 0) return 'Very Weak';
    if (metCount <= 2) return 'Weak';
    if (metCount <= 3) return 'Medium';
    if (metCount <= 4) return 'Strong';
    return 'Very Strong';
  }

  // Validate email format
  validateEmailFormat(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  // Validate organization name
  validateOrganizationName(name: string): boolean {
    const nameRegex = /^[a-zA-Z0-9 '-]+$/;
    return nameRegex.test(name);
  }

  // Go to login page
  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  // Auto-generate organization slug from name (for preview)
  get organizationSlug(): string {
    if (!this.f['organizationName']?.value) return '';
    return this.f['organizationName'].value
      .toLowerCase()
      .replace(/[^a-z0-9]/g, '-')
      .replace(/-+/g, '-')
      .replace(/^-|-$/g, '');
  }
}