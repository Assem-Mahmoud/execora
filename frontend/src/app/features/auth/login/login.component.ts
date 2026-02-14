import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../core/models/auth.model';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  rememberMe = false;

  private subscription: Subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [
        Validators.required,
        Validators.email,
        Validators.maxLength(255),
        Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(12),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#^~\\-_=.])[A-Za-z\\d@$!%*?&#^~\\-_=.]{12,}$')
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
    return this.loginForm.controls;
  }

  // Submit login form
  onSubmit(): void {
    this.submitted = true;

    // Stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    // Call API to login user
    const loginData: LoginRequest = this.loginForm.value;

    const subscription = this.authService.login(loginData).pipe(
      tap((user: User) => {
        // The auth service already stores the tokens and user data
        if (this.rememberMe) {
          localStorage.setItem('execora_remember_me', 'true');
        }
      })
    ).subscribe({
      next: () => {
        this.snackBar.open('Login successful!', 'Close', { duration: 3000 });
        // Redirect to dashboard
        this.router.navigate(['/dashboard']);
      },
      error: (error: any) => {
        this.loading = false;
        this.snackBar.open(error?.error?.message || error?.message || 'Login failed. Please check your credentials and try again.', 'Close', { duration: 5000 });
        console.error('Login error:', error);
      }
    });

    this.subscription.add(subscription);
  }

  // Forgot password
  forgotPassword(): void {
    this.router.navigate(['/auth/forgot-password']);
  }

  // Go to register
  goToRegister(): void {
    this.router.navigate(['/register']);
  }

  // Handle Enter key press
  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.onSubmit();
    }
  }
}