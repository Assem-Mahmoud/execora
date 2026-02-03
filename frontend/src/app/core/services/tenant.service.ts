import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Tenant } from '../models';

/**
 * Create tenant request
 */
export interface CreateTenantRequest {
  name: string;
  slug: string;
}

/**
 * Update tenant request
 */
export interface UpdateTenantRequest {
  name?: string;
  slug?: string;
}

/**
 * Tenant service for tenant operations
 */
@Injectable({
  providedIn: 'root'
})
export class TenantService {
  constructor(private apiService: ApiService) {}

  /**
   * Get current tenant details
   */
  getCurrentTenant(): Observable<Tenant> {
    return this.apiService.get<Tenant>('/app/tenants/current');
  }

  /**
   * Get tenant by ID
   */
  getTenantById(id: string): Observable<Tenant> {
    return this.apiService.get<Tenant>(`/app/tenants/${id}`);
  }

  /**
   * Get tenant by slug
   */
  getTenantBySlug(slug: string): Observable<Tenant> {
    return this.apiService.get<Tenant>(`/app/tenants/by-slug/${slug}`);
  }

  /**
   * Update current tenant
   */
  updateCurrentTenant(data: UpdateTenantRequest): Observable<Tenant> {
    return this.apiService.put<Tenant>('/app/tenants/current', data);
  }

  /**
   * Get tenant usage statistics
   */
  getUsageStats(): Observable<{
    projectCount: number;
    userCount: number;
    maxProjects: number;
    maxUsers: number;
    subscriptionExpiry: string;
  }> {
    return this.apiService.get<any>('/app/tenants/usage');
  }

  /**
   * Get subscription details
   */
  getSubscription(): Observable<{
    plan: string;
    status: string;
    expiryDate: string;
    features: string[];
  }> {
    return this.apiService.get<any>('/app/tenants/subscription');
  }
}
