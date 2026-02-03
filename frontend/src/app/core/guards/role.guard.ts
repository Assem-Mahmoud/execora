import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router,
  UrlTree
} from '@angular/router';
import { Observable, map, take } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TenantRole } from '../models';

/**
 * Role guard configuration
 */
interface RoleGuardData {
  roles: TenantRole[];
  requireAll?: boolean;
}

/**
 * Guard that checks if user has required role(s)
 * Usage in route config: { canActivate: [roleGuard], data: { roles: [TenantRole.TenantAdmin] } }
 */
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const guardData = route.data as RoleGuardData;
  const requiredRoles = guardData?.roles || [];
  const requireAll = guardData?.requireAll ?? false;

  return authService.authState$.pipe(
    take(1),
    map(authState => {
      if (!authState.isAuthenticated) {
        router.navigate(['/auth/login'], {
          queryParams: { redirect: state.url }
        });
        return false;
      }

      if (!authState.currentRole) {
        router.navigate(['/auth/select-tenant']);
        return false;
      }

      const hasAccess = checkRoleAccess(authState.currentRole, requiredRoles, requireAll);

      if (!hasAccess) {
        router.navigate(['/unauthorized']);
        return false;
      }

      return true;
    })
  );
};

/**
 * Check if user's role satisfies the required roles
 */
function checkRoleAccess(
  userRole: TenantRole,
  requiredRoles: TenantRole[],
  requireAll: boolean
): boolean {
  if (requiredRoles.length === 0) {
    return true;
  }

  if (requireAll) {
    return requiredRoles.every(role => role === userRole);
  } else {
    return requiredRoles.some(role => hasSufficientRole(userRole, role));
  }
}

/**
 * Check if user's role has sufficient privileges for the required role
 * Based on role hierarchy
 */
function hasSufficientRole(userRole: TenantRole, requiredRole: TenantRole): boolean {
  // Define role hierarchy (lower index = higher privilege)
  const roleHierarchy: Record<TenantRole, number> = {
    [TenantRole.SystemAdmin]: 1,
    [TenantRole.TenantAdmin]: 2,
    [TenantRole.ProjectAdmin]: 3,
    [TenantRole.ProjectManager]: 4,
    [TenantRole.QAQC]: 5,
    [TenantRole.SiteEngineer]: 6
  };

  const userLevel = roleHierarchy[userRole] ?? Number.MAX_VALUE;
  const requiredLevel = roleHierarchy[requiredRole] ?? Number.MAX_VALUE;

  // User has access if their level is equal to or lower (higher privilege) than required
  return userLevel <= requiredLevel;
}

/**
 * Helper factory functions for common role requirements
 */
export const tenantAdminGuard = (): CanActivateFn => {
  return (route, state) => {
    const adaptedRoute = { ...route, data: { roles: [TenantRole.TenantAdmin] } };
    return roleGuard(adaptedRoute, state);
  };
};

export const projectManagementGuard = (): CanActivateFn => {
  return (route, state) => {
    const adaptedRoute = {
      ...route,
      data: {
        roles: [TenantRole.ProjectAdmin, TenantRole.ProjectManager, TenantRole.TenantAdmin]
      }
    };
    return roleGuard(adaptedRoute, state);
  };
};

export const systemAdminGuard = (): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.authState$.pipe(
      take(1),
      map(authState => {
        if (authState.currentRole === TenantRole.SystemAdmin) {
          return true;
        }
        router.navigate(['/unauthorized']);
        return false;
      })
    );
  };
};
