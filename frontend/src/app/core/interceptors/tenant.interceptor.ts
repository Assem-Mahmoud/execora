import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * HTTP Interceptor that adds tenant identification headers to requests
 */
@Injectable()
export class TenantInterceptor implements HttpInterceptor {
  private readonly TENANT_ID_HEADER = 'X-Tenant-Id';
  private readonly TENANT_SLUG_HEADER = 'X-Tenant-Slug';

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Skip tenant headers for system admin endpoints
    if (this.isSystemAdminEndpoint(request.url)) {
      return next.handle(request);
    }

    // Skip tenant headers for public endpoints
    if (this.isPublicEndpoint(request.url)) {
      return next.handle(request);
    }

    // Add tenant headers if user is authenticated
    const currentTenant = this.authService.currentTenant;
    if (currentTenant) {
      request = request.clone({
        setHeaders: {
          [this.TENANT_ID_HEADER]: currentTenant.id,
          [this.TENANT_SLUG_HEADER]: currentTenant.slug
        }
      });
    }

    return next.handle(request);
  }

  /**
   * Check if the endpoint is a system admin endpoint
   */
  private isSystemAdminEndpoint(url: string): boolean {
    return url.includes('/api/sys/');
  }

  /**
   * Check if the endpoint is public (doesn't require tenant context)
   */
  private isPublicEndpoint(url: string): boolean {
    const publicEndpoints = ['/api/auth/', '/health', '/swagger', '/assets'];
    return publicEndpoints.some(endpoint => url.includes(endpoint));
  }
}
