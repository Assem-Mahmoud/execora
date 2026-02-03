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
 * Guard that checks if user has selected a tenant
 * Redirects to tenant selection page if no tenant is selected
 */
export const tenantGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.authState$.pipe(
    take(1),
    map(authState => {
      if (!authState.isAuthenticated) {
        router.navigate(['/auth/login'], {
          queryParams: { redirect: state.url }
        });
        return false;
      }

      if (!authState.currentTenant) {
        // User has multiple tenants, need to select one
        if (authState.allTenants.length > 1) {
          router.navigate(['/auth/select-tenant'], {
            queryParams: { redirect: state.url }
          });
        } else if (authState.allTenants.length === 1) {
          // Auto-select the only tenant
          authService.switchTenant(authState.allTenants[0].id).subscribe();
          return true;
        } else {
          // User has no tenant access
          router.navigate(['/no-tenant']);
        }
        return false;
      }

      return true;
    })
  );
};
