import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, switchMap, tap, catchError } from 'rxjs/operators';
import { ApiService } from './api.service';
import { User, UserProfile, Tenant, TenantRole } from '../models';

/**
 * Authentication tokens
 */
export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

/**
 * Login request
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Register request
 */
export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

/**
 * Token response from API
 */
interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserProfile;
  tenant: Tenant;
  role: TenantRole;
}

/**
 * Current authenticated user state
 */
interface AuthState {
  isAuthenticated: boolean;
  user: UserProfile | null;
  currentTenant: Tenant | null;
  currentRole: TenantRole | null;
  allTenants: Tenant[];
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly ACCESS_TOKEN_KEY = 'execora_access_token';
  private readonly REFRESH_TOKEN_KEY = 'execora_refresh_token';
  private readonly USER_KEY = 'execora_user';
  private readonly TENANT_KEY = 'execora_tenant';
  private readonly ROLE_KEY = 'execora_role';

  private authState$ = new BehaviorSubject<AuthState>({
    isAuthenticated: false,
    user: null,
    currentTenant: null,
    currentRole: null,
    allTenants: []
  });

  authState$ = this.authState$.asObservable();

  constructor(private apiService: ApiService) {
    this.initializeFromStorage();
  }

  /**
   * Get current access token
   */
  get accessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get current authenticated user
   */
  get currentUser(): UserProfile | null {
    return this.authState$.value.user;
  }

  /**
   * Get current tenant
   */
  get currentTenant(): Tenant | null {
    return this.authState$.value.currentTenant;
  }

  /**
   * Get current role in tenant
   */
  get currentRole(): TenantRole | null {
    return this.authState$.value.currentRole;
  }

  /**
   * Check if user is authenticated
   */
  get isAuthenticated(): boolean {
    return this.authState$.value.isAuthenticated;
  }

  /**
   * Check if user has specific role
   */
  hasRole(role: TenantRole): boolean {
    return this.authState$.value.currentRole === role;
  }

  /**
   * Check if user has any of the specified roles
   */
  hasAnyRole(roles: TenantRole[]): boolean {
    return roles.includes(this.authState$.value.currentRole!);
  }

  /**
   * Login with email and password
   */
  login(credentials: LoginRequest): Observable<UserProfile> {
    return this.apiService.post<TokenResponse>('/auth/login', credentials).pipe(
      tap(response => this.storeTokens(response)),
      map(response => response.user),
      tap(user => this.updateAuthState({
        isAuthenticated: true,
        user,
        currentTenant: this.getStoredTenant(),
        currentRole: this.getStoredRole(),
        allTenants: user.tenantUsers.map(tu => tu.tenant!).filter(t => t)
      }))
    );
  }

  /**
   * Register a new user
   */
  register(data: RegisterRequest): Observable<UserProfile> {
    return this.apiService.post<TokenResponse>('/auth/register', data).pipe(
      tap(response => this.storeTokens(response)),
      map(response => response.user),
      tap(user => this.updateAuthState({
        isAuthenticated: true,
        user,
        currentTenant: this.getStoredTenant(),
        currentRole: this.getStoredRole(),
        allTenants: user.tenantUsers.map(tu => tu.tenant!).filter(t => t)
      }))
    );
  }

  /**
   * Refresh access token
   */
  refreshToken(): Observable<AuthTokens> {
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    return this.apiService.post<{ accessToken: string; expiresIn: number }>('/auth/refresh', { refreshToken }).pipe(
      tap(response => {
        localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
      }),
      map(response => ({
        accessToken: response.accessToken,
        refreshToken,
        expiresIn: response.expiresIn
      }))
    );
  }

  /**
   * Logout current user
   */
  logout(): Observable<void> {
    return this.apiService.post<void>('/auth/logout', {}).pipe(
      tap(() => this.clearAuth()),
      catchError(() => {
        this.clearAuth();
        return of(void 0);
      })
    );
  }

  /**
   * Switch to a different tenant
   */
  switchTenant(tenantId: string): Observable<boolean> {
    const user = this.currentUser;
    if (!user) {
      return throwError(() => new Error('User not authenticated'));
    }

    const tenantUser = user.tenantUsers.find(tu => tu.tenantId === tenantId);
    if (!tenantUser) {
      return throwError(() => new Error('Tenant not found for user'));
    }

    // Store new tenant and role
    localStorage.setItem(this.TENANT_KEY, JSON.stringify(tenantUser.tenant));
    localStorage.setItem(this.ROLE_KEY, tenantUser.role);

    this.updateAuthState({
      ...this.authState$.value,
      currentTenant: tenantUser.tenant!,
      currentRole: tenantUser.role
    });

    return of(true);
  }

  /**
   * Initialize auth state from storage
   */
  private initializeFromStorage(): void {
    const userStr = localStorage.getItem(this.USER_KEY);
    const tenant = this.getStoredTenant();
    const role = this.getStoredRole();

    if (userStr && this.accessToken) {
      try {
        const user: UserProfile = JSON.parse(userStr);
        this.updateAuthState({
          isAuthenticated: true,
          user,
          currentTenant: tenant,
          currentRole: role,
          allTenants: user.tenantUsers.map(tu => tu.tenant!).filter(t => t)
        });
      } catch {
        this.clearAuth();
      }
    }
  }

  /**
   * Store authentication tokens and user data
   */
  private storeTokens(response: TokenResponse): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(this.TENANT_KEY, JSON.stringify(response.tenant));
    localStorage.setItem(this.ROLE_KEY, response.role);
  }

  /**
   * Get stored tenant from localStorage
   */
  private getStoredTenant(): Tenant | null {
    const tenantStr = localStorage.getItem(this.TENANT_KEY);
    return tenantStr ? JSON.parse(tenantStr) : null;
  }

  /**
   * Get stored role from localStorage
   */
  private getStoredRole(): TenantRole | null {
    const roleStr = localStorage.getItem(this.ROLE_KEY);
    return roleStr ? roleStr as TenantRole : null;
  }

  /**
   * Update authentication state
   */
  private updateAuthState(state: AuthState): void {
    this.authState$.next(state);
  }

  /**
   * Clear all authentication data
   */
  private clearAuth(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TENANT_KEY);
    localStorage.removeItem(this.ROLE_KEY);

    this.updateAuthState({
      isAuthenticated: false,
      user: null,
      currentTenant: null,
      currentRole: null,
      allTenants: []
    });
  }
}
