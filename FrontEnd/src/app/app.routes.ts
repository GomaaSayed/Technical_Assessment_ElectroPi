import { Routes } from '@angular/router';
import { authGuard } from './shared/auth.guard';
import { roleGuard } from './shared/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then((m) => m.LoginComponent),
  },

  {
    path: 'customer',
    canActivate: [authGuard, roleGuard(['Customer'])],
    loadComponent: () =>
      import('./pages/customer/customer.component').then(
        (m) => m.CustomerComponent,
      ),
  },

  {
    path: 'supportagent',
    canActivate: [authGuard, roleGuard(['SupportAgent'])],
    loadComponent: () =>
      import('./pages/support-agent/support-agent.component').then(
        (m) => m.SupportAgentComponent,
      ),
  },

  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['Admin'])],
    loadComponent: () =>
      import('./pages/admin/admin.component').then((m) => m.AdminComponent),
  },

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },

  {
    path: '**',
    redirectTo: 'login',
  },
];
