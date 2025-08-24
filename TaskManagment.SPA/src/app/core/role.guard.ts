import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const allowed = route.data?.['roles'] as string[] | undefined;
  if (!allowed) return true;
  const r = auth.role();
  if (r && allowed.includes(r)) return true;
  router.navigate(['/dashboard']);
  return false;
};
