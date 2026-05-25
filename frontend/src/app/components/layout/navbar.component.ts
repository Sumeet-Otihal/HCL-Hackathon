import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ButtonComponent } from '../ui/button.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonComponent],
  template: `
    <nav class="bg-navy-900 text-white sticky top-0 z-50 shadow-md">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <a routerLink="/" class="flex items-center space-x-2">
              <span class="text-xl font-bold tracking-tight text-white hover:text-gold-500 transition-colors">LuxeStay</span>
            </a>
          </div>

          <div class="hidden md:flex items-center space-x-8">
            <a routerLink="/hotels" class="hover:text-gold-500 transition-colors text-sm font-medium">Find Hotels</a>
            
            <ng-container *ngIf="auth.isAuthenticated()">
              <a [routerLink]="getDashboardLink()" class="hover:text-gold-500 transition-colors text-sm font-medium">Dashboard</a>
              <div class="flex items-center space-x-4 border-l border-navy-700 pl-8">
                <div class="flex items-center space-x-2">
                  <span class="text-sm font-medium text-gold-500">{{ auth.user()?.firstName }}</span>
                </div>
                <app-button variant="ghost" size="sm" class="text-white hover:text-gold-500 border border-navy-700" (onClick)="logout()">
                  Logout
                </app-button>
              </div>
            </ng-container>

            <ng-container *ngIf="!auth.isAuthenticated()">
              <div class="flex items-center space-x-4">
                <a routerLink="/login" class="hover:text-gold-500 transition-colors text-sm font-medium">Login</a>
                <a routerLink="/register">
                  <app-button size="sm">Register</app-button>
                </a>
              </div>
            </ng-container>
          </div>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  auth = inject(AuthService);

  logout() {
    this.auth.logout();
  }

  getDashboardLink(): string {
    const user = this.auth.user();
    if (!user) return '/';
    switch (user.role) {
      case 'SuperAdmin': return '/dashboard/super-admin';
      case 'HotelAdmin': return '/dashboard/hotel-admin';
      default: return '/dashboard/user';
    }
  }
}
