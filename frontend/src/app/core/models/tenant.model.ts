/**
 * Subscription plans available in EXECORA
 */
export enum SubscriptionPlan {
  Core = 1,
  Professional = 2,
  Enterprise = 3
}

/**
 * Subscription status for tenant billing
 */
export enum SubscriptionStatus {
  Active = 'Active',
  Suspended = 'Suspended',
  Trial = 'Trial',
  PastDue = 'PastDue'
}

/**
 * Tenant role assignments
 */
export enum TenantRole {
  SystemAdmin = 'SystemAdmin',
  TenantAdmin = 'TenantAdmin',
  ProjectAdmin = 'ProjectAdmin',
  ProjectManager = 'ProjectManager',
  QAQC = 'QAQC',
  SiteEngineer = 'SiteEngineer'
}

/**
 * Represents an organization/customer in the multi-tenant system
 */
export interface Tenant {
  id: string;
  name: string;
  slug: string;
  subscriptionPlan: SubscriptionPlan;
  subscriptionStatus: SubscriptionStatus;
  subscriptionExpiry?: string | null;
  maxProjects?: number | null;
  maxUsers?: number | null;
  createdAt: string;
  updatedAt: string;
}
