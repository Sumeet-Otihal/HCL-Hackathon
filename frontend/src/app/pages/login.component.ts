import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ButtonComponent } from '../components/ui/button.component';
import { InputComponent } from '../components/ui/input.component';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, ButtonComponent, InputComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-slate-50 px-4">
      <div class="max-w-md w-full">
        <div class="text-center mb-10">
          
          <h1 class="text-3xl font-bold text-navy-900">Welcome Back</h1>
          <p class="text-navy-500 mt-2">Login to manage your luxury stays</p>
        </div>

        <div class="bg-white p-8 rounded-2xl shadow-sm border border-slate-100">
          <div *ngIf="error" class="mb-6 p-3 bg-red-50 text-red-500 text-sm rounded-lg border border-red-100">
            {{ error }}
          </div>

          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-6">
            <app-input 
              label="Email Address" 
              type="email" 
              placeholder="name@example.com"
              formControlName="email"
              [error]="(loginForm.get('email')?.touched && loginForm.get('email')?.invalid) ? 'Invalid email address' : null"
            ></app-input>
            
            <app-input 
              label="Password" 
              type="password" 
              placeholder="••••••••"
              formControlName="password"
              [error]="(loginForm.get('password')?.touched && loginForm.get('password')?.invalid) ? 'Password is required' : null"
            ></app-input>
            
            <div class="flex items-center justify-between">
              <label class="flex items-center cursor-pointer">
                <input type="checkbox" class="h-4 w-4 text-gold-500 rounded border-slate-300 focus:ring-gold-500" />
                <span class="ml-2 text-sm text-navy-600">Remember me</span>
              </label>
              <a routerLink="/forgot-password" class="text-sm font-medium text-gold-600 hover:text-gold-500">
                Forgot password?
              </a>
            </div>

            <app-button type="submit" class="w-full h-11 block" [isLoading]="isSubmitting" [disabled]="loginForm.invalid">
              Sign In
            </app-button>
          </form>

          <div class="mt-8 pt-8 border-t border-slate-100 text-center">
            <p class="text-sm text-navy-600">
              Don't have an account? 
              <a routerLink="/register" class="font-bold text-gold-600 hover:text-gold-500">
                Create an account
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginPageComponent {
  loginForm: FormGroup;
  error: string | null = null;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.isSubmitting = true;
    this.error = null;

    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const user = res.data.user;
          if (user.role === 'SuperAdmin') this.router.navigate(['/dashboard/super-admin']);
          else if (user.role === 'HotelAdmin') this.router.navigate(['/dashboard/hotel-admin']);
          else this.router.navigate(['/dashboard/user']);
        }
        this.isSubmitting = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Invalid email or password';
        this.isSubmitting = false;
      }
    });
  }
}
