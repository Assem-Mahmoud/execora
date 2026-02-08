import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm: FormGroup;
  isLoading = false;
  submitted = false;
  passwordVisible = {
    current: false,
    new: false,
    confirm: false
  };

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [
        Validators.required,
        Validators.minLength(8)
      ]],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(12),
        Validators.maxLength(100),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$/)
      ]],
      confirmPassword: ['', [
        Validators.required
      ]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // Check if user is not authenticated
    if (!this.authService.isAuthenticated) {
      this.router.navigate(['/auth/login']);
    }
  }

  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const newPassword = group.get('newPassword');
    const confirmPassword = group.get('confirmPassword');

    if (newPassword && confirmPassword && newPassword.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }

    return null;
  }

  get f() {
    return this.changePasswordForm.controls;
  }

  togglePasswordVisibility(field: 'current' | 'new' | 'confirm'): void {
    this.passwordVisible[field] = !this.passwordVisible[field];
  }

  getPasswordStrengthClass(): string {
    const password = this.f['newPassword'].value;
    if (!password || password.length === 0) return '';

    if (password.length < 8) return 'weak';

    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecial = /[^a-zA-Z\d]/.test(password);

    const conditions = [hasUpperCase, hasLowerCase, hasNumbers, hasSpecial];
    const metConditions = conditions.filter(condition => condition).length;

    if (metConditions <= 2) return 'weak';
    if (metConditions === 3) return 'medium';
    return 'strong';
  }

  getPasswordStrengthText(): string {
    const strengthClass = this.getPasswordStrengthClass();
    switch (strengthClass) {
      case 'weak':
        return 'Weak';
      case 'medium':
        return 'Medium';
      case 'strong':
        return 'Strong';
      default:
        return '';
    }
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.changePasswordForm.invalid) {
      return;
    }

    this.isLoading = true;

    const changePasswordRequest = {
      currentPassword: this.f['currentPassword'].value,
      newPassword: this.f['newPassword'].value,
      confirmPassword: this.f['confirmPassword'].value
    };

    this.authService.changePassword(changePasswordRequest).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.snackBar.open('Password changed successfully. Please log in with your new password.', 'OK', {
          duration: 5000
        });

        // Redirect to login after showing message
        setTimeout(() => {
          this.authService.logout().subscribe(() => {
            this.router.navigate(['/auth/login']);
          });
        }, 2000);
      },
      error: (error) => {
        this.isLoading = false;
        let errorMessage = 'An error occurred while changing your password.';

        if (error.error?.message) {
          errorMessage = error.error.message;
        }

        this.snackBar.open(errorMessage, 'OK', {
          duration: 5000
        });
      }
    });
  }
}