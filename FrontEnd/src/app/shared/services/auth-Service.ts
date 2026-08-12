import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { ConfigService } from './config-service';

interface LoginDTO {
  username: string;
  password: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private config: any;

  private roleSubject = new BehaviorSubject<string | null>(
    localStorage.getItem('role'),
  );

  role$ = this.roleSubject.asObservable();

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
  ) {
    this.configService.getConfig().subscribe({
      next: (config) => {
        this.config = config;
      },
      error: (error) => {
        console.error('Failed to load application config', error);
      },
    });
  }

  // =========================
  // Login
  // =========================

  login(loginData: LoginDTO): Observable<any> {
    return this.http.post(
      `${this.config.baseUrl}Authentication/login`,
      loginData,
    );
  }

  // =========================
  // Save Authentication
  // =========================

  saveAuth(response: any): void {
    const token = response?.token;

    if (!token) {
      return;
    }

    localStorage.setItem('token', token);

    const role = this.getRoleFromToken(token);

    if (role) {
      localStorage.setItem('role', role);
      this.roleSubject.next(role);
    }
  }

  // =========================
  // Authentication
  // =========================

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');

    return !!token;
  }

  // =========================
  // Role
  // =========================

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  // =========================
  // Home Route
  // =========================

  getHomeRoute(): string {
    const role = this.getRole()?.toLowerCase();

    switch (role) {
      case 'admin':
        return '/admin';

      case 'supportagent':
        return '/supportagent';

      case 'customer':
        return '/customer';

      default:
        return '/login';
    }
  }

  // =========================
  // Logout
  // =========================

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');

    this.roleSubject.next(null);
  }

  // =========================
  // JWT Role
  // =========================

  private getRoleFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      const role =
        payload[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ||
        payload.role ||
        payload.Role;

      return Array.isArray(role) ? role[0] : (role ?? null);
    } catch (error) {
      console.error('Invalid JWT token', error);
      return null;
    }
  }
}
