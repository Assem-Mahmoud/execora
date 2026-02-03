import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

/**
 * API response wrapper
 */
export interface ApiResponse<T> {
  data: T;
  message?: string;
  errors?: Record<string, string[]>;
}

/**
 * Paginated response
 */
export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * API error response
 */
export interface ApiError {
  type: string;
  message: string;
  code: string;
  statusCode: number;
  stackTrace?: string;
  innerException?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = '/api';

  constructor(private http: HttpClient) {}

  /**
   * Perform GET request
   */
  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}${endpoint}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform GET request with wrapped response
   */
  getWrapped<T>(endpoint: string, params?: HttpParams): Observable<ApiResponse<T>> {
    return this.http.get<ApiResponse<T>>(`${this.apiUrl}${endpoint}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform POST request
   */
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform POST request with wrapped response
   */
  postWrapped<T>(endpoint: string, body: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform PUT request
   */
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform PATCH request
   */
  patch<T>(endpoint: string, body: any): Observable<T> {
    return this.http.patch<T>(`${this.apiUrl}${endpoint}`, body).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Perform DELETE request
   */
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}${endpoint}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Upload file
   */
  upload<T>(endpoint: string, file: File, additionalData?: Record<string, any>): Observable<T> {
    const formData = new FormData();
    formData.append('file', file);

    if (additionalData) {
      Object.keys(additionalData).forEach(key => {
        formData.append(key, additionalData[key]);
      });
    }

    return this.http.post<T>(`${this.apiUrl}${endpoint}`, formData).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: any): Observable<never> {
    const apiError: ApiError = error.error || {
      type: 'Error',
      message: 'An unexpected error occurred',
      code: 'INTERNAL_ERROR',
      statusCode: error.status || 500
    };

    console.error('API Error:', apiError);
    return throwError(() => apiError);
  }

  /**
   * Build HTTP params from object
   */
  buildParams(params: Record<string, any>): HttpParams {
    let httpParams = new HttpParams();

    Object.keys(params).forEach(key => {
      const value = params[key];
      if (value !== null && value !== undefined) {
        if (Array.isArray(value)) {
          value.forEach(v => httpParams = httpParams.append(key, String(v)));
        } else {
          httpParams = httpParams.set(key, String(value));
        }
      }
    });

    return httpParams;
  }
}
