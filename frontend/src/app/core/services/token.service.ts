import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, from, of, throwError } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';

/**
 * Service for managing JWT tokens with automatic refresh
 */
@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'execora_access_token';
  private readonly REFRESH_TOKEN_KEY = 'execora_refresh_token';
  private readonly TOKEN_REFRESH_THRESHOLD = 5 * 60 * 1000; // 5 minutes in ms

  private tokenRefreshInProgress = false;
  private refreshSubject$ = new BehaviorSubject<boolean>(false);

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  /**
   * Get current access token
   */
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get current refresh token
   */
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Store access token
   */
  storeAccessToken(token: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  /**
   * Store refresh token
   */
  storeRefreshToken(token: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, token);
  }

  /**
   * Check if token is expiring soon
   */
  isTokenExpiringSoon(): boolean {
    const token = this.getAccessToken();
    if (!token) {
      return false;
    }

    try {
      const payload = this.parseJwt(token);
      const expirationTime = payload.exp * 1000; // Convert to milliseconds
      const currentTime = Date.now();
      const timeUntilExpiration = expirationTime - currentTime;

      return timeUntilExpiration < this.TOKEN_REFRESH_THRESHOLD && timeUntilExpiration > 0;
    } catch {
      return false;
    }
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const token = this.getAccessToken();
    if (!token) {
      return true;
    }

    try {
      const payload = this.parseJwt(token);
      const expirationTime = payload.exp * 1000;
      return Date.now() >= expirationTime;
    } catch {
      return true;
    }
  }

  /**
   * Refresh access token using refresh token
   */
  refreshToken(): Observable<{ accessToken: string; refreshToken: string }> {
    if (this.tokenRefreshInProgress) {
      return this.refreshSubject$.pipe(
        switchMap(() => {
          const accessToken = this.getAccessToken();
          const refreshToken = this.getRefreshToken();
          if (!accessToken || !refreshToken) {
            return throwError(() => new Error('No tokens available'));
          }
          return of({ accessToken, refreshToken });
        })
      );
    }

    this.tokenRefreshInProgress = true;

    return this.apiService.post<{ accessToken: string; refreshToken: string; expiresIn: number }>(
      '/auth/refresh',
      { refreshToken: this.getRefreshToken() }
    ).pipe(
      tap(response => {
        this.storeAccessToken(response.accessToken);
        if (response.refreshToken) {
          this.storeRefreshToken(response.refreshToken);
        }
        this.tokenRefreshInProgress = false;
        this.refreshSubject$.next(true);
      }),
      catchError(error => {
        this.tokenRefreshInProgress = false;
        this.clearTokens();
        this.refreshSubject$.next(false);
        return throwError(() => error);
      })
    );
  }

  /**
   * Clear all tokens
   */
  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Parse JWT token to extract payload
   */
  private parseJwt(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  }
}
