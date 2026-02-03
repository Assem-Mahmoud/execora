/**
 * Represents a user in the system
 */
export interface User {
  id: string;
  email: string;
  emailConfirmed: boolean;
  phoneNumber?: string | null;
  phoneNumberConfirmed: boolean;
  firstName: string;
  lastName: string;
  isActive: boolean;
  lastLoginAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

/**
 * User's membership in a tenant
 */
export interface TenantUser {
  id: string;
  tenantId: string;
  userId: string;
  role: TenantRole;
  permissions?: string | null;
  invitedBy?: string | null;
  invitedAt?: string | null;
  joinedAt?: string | null;
  isActive: boolean;
  tenant?: Tenant;
}

/**
 * Full user profile with their tenant memberships
 */
export interface UserProfile extends User {
  tenantUsers: TenantUser[];
}
