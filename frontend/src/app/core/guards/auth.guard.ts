import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router,
  UrlTree
} from '@angular/router';
import { Observable, map, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Guard that checks if user is authenticated
 * Redirects to login page if not authenticated
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.authState$.pipe(
    take(1),
    map(authState => {
      if (authState.isAuthenticated) {
        return true;
      }

      // Store redirect URL for after login
      router.navigate(['/auth/login'], {
        queryParams: { redirect: state.url }
      });
      return false;
    })
  );
};

/**
 * Guard that checks if user is NOT authenticated
 * Redirects to dashboard if already authenticated (for login/register pages)
 */
export const nonAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.authState$.pipe(
    take(1),
    map(authState => {
      if (!authState.isAuthenticated) {
        return true;
      }

      // Already authenticated, redirect to dashboard
      router.navigate(['/app/dashboard']);
      return false;
    })
  );
};

/**
 * Type guard for Observable<boolean | UrlTree> return type
 */
type GuardResult = Observable<boolean | UrlTree> | boolean | UrlTree;
