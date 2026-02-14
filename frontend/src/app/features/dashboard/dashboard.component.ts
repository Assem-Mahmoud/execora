import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: any = null;
  isEmailVerified = false;

  constructor(
    private router: Router,
    private snackBar: MatSnackBar,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Check if user is logged in
    if (!localStorage.getItem('execora_access_token')) {
      this.snackBar.open('Please login to access the dashboard', 'Close', { duration: 5000 });
      this.router.navigate(['/login']);
      return;
    }

    // Get user data
    this.user = this.authService.currentUser;
    if (this.user) {
      this.isEmailVerified = this.user.emailVerified;
    }
  }

  logout(): void {
    localStorage.removeItem('execora_access_token');
    localStorage.removeItem('execora_refresh_token');
    localStorage.removeItem('execora_user');
    this.snackBar.open('You have been logged out', 'Close', { duration: 5000 });
    this.router.navigate(['/login']);
  }

  resendVerification(): void {
    if (this.user?.email) {
      this.authService.resendVerification(this.user.email).subscribe({
        next: () => {
          this.snackBar.open(`Verification email resent to ${this.user.email}`, 'Close', { duration: 5000 });
        },
        error: (error: any) => {
          this.snackBar.open(error?.error?.message || 'Failed to resend verification email', 'Close', { duration: 5000 });
        }
      });
    }
  }
}