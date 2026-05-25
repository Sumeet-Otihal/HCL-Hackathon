import { Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing.component';
import { HotelsPageComponent } from './pages/hotels.component';
import { HotelDetailPageComponent } from './pages/hotel-detail.component';
import { LoginPageComponent } from './pages/login.component';
import { RegisterPageComponent } from './pages/register.component';
import { BookingPageComponent } from './pages/booking.component';
import { PaymentPageComponent } from './pages/payment.component';
import { UserDashboardComponent } from './pages/user-dashboard.component';
import { HotelAdminDashboardComponent } from './pages/hotel-admin-dashboard.component';
import { SuperAdminDashboardComponent } from './pages/super-admin-dashboard.component';
import { ProfilePageComponent } from './pages/profile.component';
import { authGuard, roleGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'hotels', component: HotelsPageComponent },
  { path: 'hotels/:id', component: HotelDetailPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  
  { path: 'hotels/:id/book/:categoryId', component: BookingPageComponent, canActivate: [authGuard] },
  { path: 'payment/:bookingId', component: PaymentPageComponent, canActivate: [authGuard] },
  { path: 'dashboard/user', component: UserDashboardComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfilePageComponent, canActivate: [authGuard] },
  
  { path: 'dashboard/hotel-admin', component: HotelAdminDashboardComponent, canActivate: [roleGuard(['HotelAdmin'])] },
  { path: 'dashboard/super-admin', component: SuperAdminDashboardComponent, canActivate: [roleGuard(['SuperAdmin'])] },
  
  { path: 'dashboard', redirectTo: 'dashboard/user', pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];
