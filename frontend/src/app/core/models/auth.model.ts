import { Tenant, TenantRole } from './tenant.model';
import { User } from './user.model';

/**
 * Authentication tokens response
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
  rememberMe?: boolean;
}

/**
 * Login response
 */
export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: User;
  tenant: Tenant;
  availableTenants?: AvailableTenant[];
}

/**
 * Tenant available to user
 */
export interface AvailableTenant {
  id: string;
  name: string;
  slug: string;
  role: TenantRole;
}

/**
 * Registration request
 */
export interface RegisterRequest {
  organizationName: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

/**
 * Registration response
 */
export interface RegisterResponse {
  userId: string;
  tenantId: string;
  email: string;
  firstName: string;
  lastName: string;
  organizationName: string;
  role: string;
  emailVerified: boolean;
  emailVerificationToken?: string;
}

/**
 * Refresh token request
 */
export interface RefreshTokenRequest {
  refreshToken: string;
}

/**
 * Refresh token response
 */
export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

/**
 * Forgot password request
 */
export interface ForgotPasswordRequest {
  email: string;
}

/**
 * Reset password request
 */
export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

/**
 * Change password request
 */
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

/**
 * Verify email request
 */
export interface VerifyEmailRequest {
  token: string;
}

/**
 * Resend verification request
 */
export interface ResendVerificationRequest {
  email: string;
}

/**
 * Verify email response
 */
export interface VerifyEmailResponse {
  success: boolean;
  emailVerified: boolean;
  email?: string;
  errorMessage?: string;
  successMessage?: string;
}

/**
 * Invitation status
 */
export enum InvitationStatus {
  Pending = 1,
  Accepted = 2,
  Expired = 3,
  Cancelled = 4
}

/**
 * Invitation detail
 */
export interface Invitation {
  id: string;
  tenantId: string;
  email: string;
  role: TenantRole;
  expiresAt: string;
  status: InvitationStatus;
  invitedBy: string;
  invitedAt: string;
  acceptedAt?: string;
  tenant?: Tenant;
}

/**
 * Create invitation request
 */
export interface CreateInvitationRequest {
  email: string;
  role: TenantRole;
}

/**
 * Invitation response
 */
export interface InvitationResponse {
  id: string;
  email: string;
  role: TenantRole;
  expiresAt: string;
  status: InvitationStatus;
}

/**
 * User profile response
 */
export interface UserProfileResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  emailConfirmed: boolean;
  tenantUsers: {
    tenantId: string;
    tenant: Tenant;
    role: TenantRole;
    isActive: boolean;
  }[];
}

/**
 * Update profile request
 */
export interface UpdateProfileRequest {
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

/**
 * Update user role request
 */
export interface UpdateUserRoleRequest {
  role: TenantRole;
}
