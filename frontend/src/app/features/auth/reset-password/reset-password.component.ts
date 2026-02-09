import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSnackBarModule,
    MatToolbarModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  isLoading = false;
  submitted = false;
  token: string | null = null;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  constructor() {
    this.resetPasswordForm = this.fb.group({
      password: ['', [
        Validators.required,
        Validators.minLength(12),
        Validators.maxLength(100),
        this.validatePasswordStrength
      ]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // Get token from URL
    this.token = this.route.snapshot.queryParamMap.get('token');

    if (!this.token) {
      this.snackBar.open('Invalid password reset token.', 'OK', {
        duration: 5000
      });
      this.router.navigate(['/auth/login']);
    }

    // Check if user is already logged in
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/dashboard']);
    }
  }

  get f() {
    return this.resetPasswordForm.controls;
  }

  validatePasswordStrength(control: any): { [key: string]: any } | null {
    const value = control.value;
    const errors: { [key: string]: any } = {};

    if (value && value.length < 12) {
      errors['minLength'] = true;
    }

    if (value && !value.match(/[A-Z]/)) {
      errors['noUppercase'] = true;
    }

    if (value && !value.match(/[a-z]/)) {
      errors['noLowercase'] = true;
    }

    if (value && !value.match(/\d/)) {
      errors['noDigit'] = true;
    }

    if (value && !value.match(/[^a-zA-Z\d]/)) {
      errors['noSpecial'] = true;
    }

    return Object.keys(errors).length > 0 ? errors : null;
  }

  passwordMatchValidator(form: FormGroup): { [key: string]: any } | null {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }

    return null;
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.resetPasswordForm.invalid || !this.token) {
      return;
    }

    this.isLoading = true;

    this.authService.resetPassword({
      token: this.token,
      newPassword: this.resetPasswordForm.value.password,
      confirmPassword: this.resetPasswordForm.value.confirmPassword
    }).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.snackBar.open('Your password has been reset successfully! You can now log in with your new password.', 'OK', {
          duration: 5000
        });

        // Redirect to login
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        this.isLoading = false;
        let errorMessage = 'An error occurred while resetting your password.';

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