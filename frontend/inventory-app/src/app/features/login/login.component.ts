import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/auth.service';
import { inject } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);

  loading = false;
  errorMessage = '';
  successMessage = '';

  readonly form = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  constructor(
    private readonly authService: AuthService
  ) {}

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    const username = this.form.controls.username.value ?? '';
    const password = this.form.controls.password.value ?? '';

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(username, password).subscribe({
      next: () => {
        this.successMessage = 'Inicio de sesión exitoso.';
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Usuario o contraseña inválidos.';
        this.loading = false;
      }
    });
  }
}
