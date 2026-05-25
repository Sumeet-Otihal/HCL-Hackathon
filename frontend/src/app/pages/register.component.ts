import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ButtonComponent } from '../components/ui/button.component';
import { InputComponent } from '../components/ui/input.component';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, ButtonComponent, InputComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-slate-50 px-4 py-12">
      <div class="max-w-md w-full">
        <div class="text-center mb-10">
          
          <h1 class="text-3xl font-bold text-navy-900">Create Account</h1>
          <p class="text-navy-500 mt-2">Join LuxeStay for exclusive rewards</p>
        </div>

        <div class="bg-white p-8 rounded-2xl shadow-sm border border-slate-100">
          <div *ngIf="error" class="mb-6 p-3 bg-red-50 text-red-500 text-sm rounded-lg border border-red-100">
            {{ error }}
          </div>

          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <app-input 
                label="First Name" 
                placeholder="John"
                formControlName="firstName"
                [error]="(registerForm.get('firstName')?.touched && registerForm.get('firstName')?.invalid) ? 'First name is too short' : null"
              ></app-input>
              <app-input 
                label="Last Name" 
                placeholder="Doe"
                formControlName="lastName"
                [error]="(registerForm.get('lastName')?.touched && registerForm.get('lastName')?.invalid) ? 'Last name is too short' : null"
              ></app-input>
            </div>
            
            <app-input 
              label="Email Address" 
              type="email" 
              placeholder="john@example.com"
              formControlName="email"
              [error]="(registerForm.get('email')?.touched && registerForm.get('email')?.invalid) ? 'Invalid email address' : null"
            ></app-input>
            
            <app-input 
              label="Password" 
              type="password" 
              placeholder="••••••••"
              formControlName="password"
              [error]="(registerForm.get('password')?.touched && registerForm.get('password')?.invalid) ? 'Password does not meet requirements' : null"
            ></app-input>
            
            <app-input 
              label="Confirm Password" 
              type="password" 
              placeholder="••••••••"
              formControlName="confirmPassword"
              [error]="registerForm.errors?.['passwordMismatch'] && registerForm.get('confirmPassword')?.touched ? 'Passwords do not match' : null"
            ></app-input>
            
            <p class="text-[10px] text-navy-400 leading-tight">
              By signing up, you agree to our Terms of Service and Privacy Policy.
            </p>

            <app-button type="submit" class="w-full h-11 mt-2 block" [isLoading]="isSubmitting" [disabled]="registerForm.invalid">
              Create Account
            </app-button>
          </form>

          <div class="mt-8 pt-8 border-t border-slate-100 text-center">
            <p class="text-sm text-navy-600">
              Already have an account? 
              <a routerLink="/login" class="font-bold text-gold-600 hover:text-gold-500">
                Sign In
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterPageComponent {
  registerForm: FormGroup;
  error: string | null = null;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$/) // 1 uppercase, 1 digit, 1 special
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirm = control.get('confirmPassword')?.value;
    if (password && confirm && password !== confirm) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isSubmitting = true;
    this.error = null;

    const { confirmPassword, ...data } = this.registerForm.value;

    this.authService.register(data).subscribe({
      next: (res) => {
        if (res.success) {
          this.router.navigate(['/login'], { queryParams: { registered: 'true' } });
        }
        this.isSubmitting = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}
