import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { VerifyEmailResponse } from '../../../core/models/auth.model';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzIconModule,
    NzSpinModule,
    NzButtonModule,
    NzInputModule,
    NzAlertModule,
    RouterLink
  ],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent {
  isLoading = false;
  isSuccess = false;
  isError = false;
  errorMessage = '';
  email = '';
  token = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private message: NzMessageService
  ) {
    // Get token from URL
    const urlParams = new URLSearchParams(window.location.search);
    this.token = urlParams.get('token') || '';
    this.email = urlParams.get('email') || '';
  }

  verifyEmail(): void {
    if (!this.token) {
      this.errorMessage = 'Verification token is required';
      this.isError = true;
      return;
    }

    this.isLoading = true;
    this.isError = false;

    this.authService.verifyEmail(this.token).subscribe({
      next: (response: VerifyEmailResponse) => {
        if (response.success) {
          this.isSuccess = true;
          this.email = response.email || this.email;
          this.message.success('Email verified successfully!');

          // Redirect to login after a delay
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        } else {
          this.isError = true;
          this.errorMessage = response.errorMessage || 'Verification failed';
          this.message.error(this.errorMessage);
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.isError = true;
        this.errorMessage = error.error?.message || 'An error occurred during verification';
        this.message.error(this.errorMessage);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  resendVerification(): void {
    if (!this.email) {
      this.message.warning('Email address is required');
      return;
    }

    this.isLoading = true;
    this.authService.resendVerification(this.email).subscribe({
      next: () => {
        this.message.success(`Verification email resent to ${this.email}`);
      },
      error: (error) => {
        this.isLoading = false;
        this.message.error(error.error?.message || 'Failed to resend verification email');
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}