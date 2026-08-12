import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth-Service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  loginForm: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  get username() {
    return this.loginForm.get('username');
  }

  get password() {
    return this.loginForm.get('password');
  }

  login(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.authService.saveAuth(response);

        const role = this.authService.getRole();

        console.log('Logged in role:', role);

        switch (role?.toLowerCase()) {
          case 'customer':
            this.router.navigate(['/customer']);
            break;

          case 'supportagent':
            this.router.navigate(['/supportagent']);
            break;

          case 'admin':
            this.router.navigate(['/admin']);
            break;

          default:
            this.errorMessage = 'User role was not found.';
            this.isSubmitting = false;
            break;
        }
      },

      error: (error) => {
        console.error('Login failed:', error);

        this.errorMessage =
          error.error?.message || 'Invalid username or password.';

        this.isSubmitting = false;
      },
    });
  }

  useDemoAccount(username: string, password: string): void {
    this.loginForm.patchValue({
      username,
      password,
    });

    this.errorMessage = '';
  }
}
