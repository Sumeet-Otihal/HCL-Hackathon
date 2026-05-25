import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models';
import { ButtonComponent } from '../components/ui/button.component';
import { InputComponent } from '../components/ui/input.component';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent],
  template: `
    <div class="max-w-4xl mx-auto px-4 py-12">
      <h1 class="text-3xl font-bold text-navy-900 mb-8">Account Settings</h1>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="md:col-span-1">
          <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 text-center">
            <div class="h-24 w-24 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
              
            </div>
            <h2 class="text-xl font-bold text-navy-900">{{ auth.user()?.firstName }} {{ auth.user()?.lastName }}</h2>
            <p class="text-navy-500 text-sm mb-6">{{ auth.user()?.email }}</p>
            
            <div class="flex items-center justify-center space-x-2 text-xs font-bold uppercase tracking-widest text-gold-600 bg-gold-50 py-2 rounded-lg">
              
              <span>{{ auth.user()?.role }}</span>
            </div>

            <app-button variant="ghost" class="w-full mt-8 text-red-500 hover:bg-red-50 block" (onClick)="logout()">
               Logout
            </app-button>
          </div>
        </div>

        <div class="md:col-span-2 space-y-8">
          <section class="bg-white p-8 rounded-2xl shadow-sm border border-slate-100">
            <h3 class="text-xl font-bold mb-6">Personal Information</h3>
            <div *ngIf="success" class="mb-6 p-3 bg-green-50 text-green-600 text-sm rounded-lg border border-green-100">
              Profile updated successfully!
            </div>
            <form [formGroup]="profileForm" (ngSubmit)="onSubmit()" class="space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <app-input 
                  label="First Name" 
                  formControlName="firstName"
                  [error]="(profileForm.get('firstName')?.touched && profileForm.get('firstName')?.invalid) ? 'First name is too short' : null"
                ></app-input>
                <app-input 
                  label="Last Name" 
                  formControlName="lastName"
                  [error]="(profileForm.get('lastName')?.touched && profileForm.get('lastName')?.invalid) ? 'Last name is too short' : null"
                ></app-input>
              </div>
              <app-input label="Email Address" [value]="auth.user()?.email" [disabled]="true"></app-input>
              
              <div class="pt-4">
                <app-button type="submit" [isLoading]="isSubmitting" [disabled]="profileForm.invalid">Save Changes</app-button>
              </div>
            </form>
          </section>
        </div>
      </div>
    </div>
  `
})
export class ProfilePageComponent implements OnInit {
  profileForm: FormGroup;
  success = false;
  isSubmitting = false;

  constructor(
    public auth: AuthService,
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit() {
    const user = this.auth.user();
    if (user) {
      this.profileForm.patchValue({
        firstName: user.firstName,
        lastName: user.lastName
      });
    }
  }

  logout() {
    this.auth.logout();
  }

  onSubmit() {
    if (this.profileForm.invalid) return;

    this.isSubmitting = true;
    this.success = false;

    this.http.put<ApiResponse<any>>(`${environment.apiUrl}/auth/me`, this.profileForm.value)
      .subscribe({
        next: (res) => {
          if (res.success && this.auth.user()) {
            const updatedUser = { ...this.auth.user()!, ...this.profileForm.value };
            this.auth.setAuth(updatedUser, this.auth.accessToken()!, this.auth.refreshToken()!);
            this.success = true;
          }
          this.isSubmitting = false;
        },
        error: () => {
          this.isSubmitting = false;
        }
      });
  }
}
