import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './services/auth-Service';

export function roleGuard(allowedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const role = authService.getRole();

    if (
      role &&
      allowedRoles.some(
        (allowedRole) => allowedRole.toLowerCase() === role.toLowerCase(),
      )
    ) {
      return true;
    }

    // User is logged in but doesn't have permission
    switch (role?.toLowerCase()) {
      case 'customer':
        return router.createUrlTree(['/customer']);

      case 'supportagent':
        return router.createUrlTree(['/supportagent']);

      case 'admin':
        return router.createUrlTree(['/admin']);

      default:
        return router.createUrlTree(['/login']);
    }
  };
}
