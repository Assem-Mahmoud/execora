import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, timer } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

/**
 * Error response structure from API
 */
interface ApiErrorResponse {
  type: string;
  message: string;
  code: string;
  statusCode: number;
}

/**
 * HTTP Interceptor that handles API errors globally
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private readonly MAX_RETRIES = 2;
  private readonly RETRY_DELAY_MS = 1000;

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      retry({
        count: this.MAX_RETRIES,
        delay: (error: HttpErrorResponse, retryCount: number) => {
          // Only retry on network errors or 5xx server errors
          if (this.shouldRetry(error)) {
            console.warn(`Retrying request (${retryCount}/${this.MAX_RETRIES}):`, request.url);
            return timer(this.RETRY_DELAY_MS * retryCount);
          }
          return throwError(() => error);
        }
      }),
      catchError((error: HttpErrorResponse) => this.handleError(error, request))
    );
  }

  /**
   * Determine if request should be retried
   */
  private shouldRetry(error: HttpErrorResponse): boolean {
    // Retry on network errors (status 0)
    if (error.status === 0) {
      return true;
    }

    // Retry on 5xx server errors
    if (error.status >= 500 && error.status < 600) {
      return true;
    }

    // Retry on 429 Too Many Requests
    if (error.status === 429) {
      return true;
    }

    return false;
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse, request: HttpRequest<unknown>): Observable<never> {
    let errorMessage = 'An unexpected error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side or network error
      errorMessage = `Network error: ${error.error.message}`;
    } else {
      // Server-side error
      const apiError = error.error as ApiErrorResponse;
      if (apiError?.message) {
        errorMessage = apiError.message;
      } else {
        switch (error.status) {
          case 400:
            errorMessage = 'Invalid request. Please check your input.';
            break;
          case 401:
            errorMessage = 'You are not authenticated. Please login.';
            break;
          case 403:
            errorMessage = 'You do not have permission to access this resource.';
            break;
          case 404:
            errorMessage = 'The requested resource was not found.';
            break;
          case 409:
            errorMessage = 'This resource already exists or conflicts with existing data.';
            break;
          case 422:
            errorMessage = 'The request could not be processed.';
            break;
          case 429:
            errorMessage = 'Too many requests. Please try again later.';
            break;
          case 500:
            errorMessage = 'A server error occurred. Please try again later.';
            break;
          case 503:
            errorMessage = 'The service is temporarily unavailable. Please try again later.';
            break;
          default:
            errorMessage = `Server error (${error.status}): ${error.statusText}`;
        }
      }
    }

    console.error('HTTP Error:', {
      url: request.url,
      status: error.status,
      message: errorMessage,
      error: error.error
    });

    return throwError(() => ({
      ...error,
      userMessage: errorMessage
    }));
  }
}
