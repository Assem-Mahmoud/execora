import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzButtonModule,
    NzIconModule,
    NzAlertModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: any = null;
  isEmailVerified = false;

  constructor(
    private router: Router,
    private message: NzMessageService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Check if user is logged in
    if (!localStorage.getItem('execora_access_token')) {
      this.message.warning('Please login to access the dashboard');
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
    this.message.success('You have been logged out');
    this.router.navigate(['/login']);
  }

  resendVerification(): void {
    if (this.user?.email) {
      this.authService.resendVerification(this.user.email).subscribe({
        next: () => {
          this.message.success(`Verification email resent to ${this.user.email}`);
        },
        error: (error: any) => {
          this.message.error(error?.error?.message || 'Failed to resend verification email');
        }
      });
    }
  }
}