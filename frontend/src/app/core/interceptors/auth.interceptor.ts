import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

/**
 * HTTP Interceptor that adds JWT authentication tokens to requests
 * and handles token refresh when tokens expire
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private readonly AUTH_HEADER_KEY = 'Authorization';
  private readonly AUTH_HEADER_VALUE_PREFIX = 'Bearer ';

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Skip auth for public endpoints
    if (this.isPublicEndpoint(request.url)) {
      return next.handle(request);
    }

    return from(this.handleRequestWithAuth(request, next));
  }

  private async handleRequestWithAuth(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Promise<HttpEvent<unknown>> {
    let authRequest = request;

    // Add access token if available
    const accessToken = this.authService.accessToken;
    if (accessToken) {
      authRequest = request.clone({
        setHeaders: {
          [this.AUTH_HEADER_KEY]: this.AUTH_HEADER_VALUE_PREFIX + accessToken
        }
      });
    }

    try {
      return await firstValueFrom(next.handle(authRequest));
    } catch (error: any) {
      // Try to refresh token on 401 error
      if (error instanceof HttpErrorResponse && error.status === 401 && accessToken) {
        try {
          const newTokens = await firstValueFrom(this.authService.refreshToken());
          // Retry original request with new token
          const retryRequest = request.clone({
            setHeaders: {
              [this.AUTH_HEADER_KEY]: this.AUTH_HEADER_VALUE_PREFIX + newTokens.accessToken
            }
          });
          return await firstValueFrom(next.handle(retryRequest));
        } catch {
          // Refresh failed, logout and continue with error
          this.authService.logout().subscribe();
          throw error;
        }
      }
      throw error;
    }
  }

  /**
   * Check if the endpoint is public (doesn't require authentication)
   */
  private isPublicEndpoint(url: string): boolean {
    const publicEndpoints = ['/api/auth/login', '/api/auth/register', '/health', '/swagger'];
    return publicEndpoints.some(endpoint => url.includes(endpoint));
  }
}

// Helper function to convert Observable to Promise
function firstValueFrom<T>(observable: Observable<T>): Promise<T> {
  return new Promise((resolve, reject) => {
    const subscription = observable.subscribe({
      next: value => {
        subscription.unsubscribe();
        resolve(value);
      },
      error: error => {
        subscription.unsubscribe();
        reject(error);
      }
    });
  });
}
