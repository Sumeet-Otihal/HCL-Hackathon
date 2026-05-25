import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RoomCategory, Booking, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { AuthService } from '../services/auth.service';
import { ButtonComponent } from '../components/ui/button.component';
import { BadgeComponent } from '../components/ui/badge.component';

@Component({
  selector: 'app-hotel-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, BadgeComponent],
  template: `
    <div class="max-w-7xl mx-auto px-4 py-12">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-navy-900">Hotel Management</h1>
        <app-button>
           Add {{ activeTab === 'categories' ? 'Category' : 'Room' }}
        </app-button>
      </div>

      <div class="flex space-x-8 mb-8 border-b border-slate-200">
        <button 
          *ngFor="let tab of ['categories', 'rooms', 'bookings']"
          (click)="activeTab = tab"
          [ngClass]="{
            'pb-4 text-sm font-bold uppercase tracking-wider transition-colors border-b-2': true,
            'border-gold-500 text-gold-500': activeTab === tab,
            'border-transparent text-navy-500 hover:text-navy-900': activeTab !== tab
          }"
        >
          {{ tab }}
        </button>
      </div>

      <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
        <ng-container *ngIf="activeTab === 'categories'">
          <table class="w-full text-left">
            <thead class="bg-slate-50 border-b border-slate-100">
              <tr>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Category</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Price</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Occupancy</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr *ngFor="let cat of categories" class="hover:bg-slate-50 transition-colors">
                <td class="px-6 py-4">
                  <div class="flex items-center">
                    <img [src]="cat.imageUrl" class="h-10 w-10 rounded object-cover mr-3" />
                    <span class="font-medium text-navy-900">{{ cat.name }}</span>
                  </div>
                </td>
                <td class="px-6 py-4 text-navy-600 font-medium">₹{{ cat.basePrice | number:'1.2-2' }}</td>
                <td class="px-6 py-4 text-navy-600">{{ cat.maxOccupancy }} Guests</td>
                <td class="px-6 py-4 text-right space-x-3">
                  <button class="text-navy-400 hover:text-navy-900"></button>
                  <button class="text-navy-400 hover:text-red-500"></button>
                </td>
              </tr>
            </tbody>
          </table>
        </ng-container>

        <ng-container *ngIf="activeTab === 'bookings'">
          <table class="w-full text-left">
            <thead class="bg-slate-50 border-b border-slate-100">
              <tr>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Guest</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Dates</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider">Status</th>
                <th class="px-6 py-4 text-sm font-bold text-navy-900 uppercase tracking-wider text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr *ngFor="let booking of bookings" class="hover:bg-slate-50 transition-colors">
                <td class="px-6 py-4">
                  <p class="font-medium text-navy-900">{{ booking.user?.firstName }} {{ booking.user?.lastName }}</p>
                  <p class="text-xs text-navy-500">{{ booking.user?.email }}</p>
                </td>
                <td class="px-6 py-4 text-sm text-navy-600">
                  {{ booking.checkInDate | date:'dd MMM yyyy' }} - {{ booking.checkOutDate | date:'dd MMM yyyy' }}
                </td>
                <td class="px-6 py-4">
                  <app-badge [variant]="getVariant(booking.status)">
                    {{ booking.status }}
                  </app-badge>
                </td>
                <td class="px-6 py-4 text-right">
                  <select 
                    class="text-xs border rounded p-1 bg-white"
                    [ngModel]="booking.status"
                    (ngModelChange)="updateBookingStatus(booking.id, $event)"
                  >
                    <option value="Pending">Pending</option>
                    <option value="Confirmed">Confirmed</option>
                    <option value="Cancelled">Cancelled</option>
                    <option value="Completed">Completed</option>
                  </select>
                </td>
              </tr>
            </tbody>
          </table>
        </ng-container>
      </div>
    </div>
  `
})
export class HotelAdminDashboardComponent implements OnInit {
  activeTab = 'categories';
  categories: RoomCategory[] = [];
  bookings: Booking[] = [];
  hotelId: string | undefined;

  constructor(private http: HttpClient, private authService: AuthService) {
    this.hotelId = this.authService.user()?.hotelId;
  }

  ngOnInit() {
    if (this.hotelId) {
      this.fetchData();
    }
  }

  fetchData() {
    this.http.get<ApiResponse<RoomCategory[]>>(`${environment.apiUrl}/rooms/hotel/${this.hotelId}/categories`)
      .subscribe(res => this.categories = res.data || []);
      
    this.http.get<ApiResponse<Booking[]>>(`${environment.apiUrl}/bookings/hotel/${this.hotelId}`)
      .subscribe(res => this.bookings = res.data || []);
  }

  updateBookingStatus(id: string, status: string) {
    this.http.patch<ApiResponse<any>>(`${environment.apiUrl}/bookings/${id}/status`, { status })
      .subscribe(res => {
        if (res.success) {
          const booking = this.bookings.find(b => b.id === id);
          if (booking) booking.status = status as any;
        }
      });
  }

  getVariant(status: string): 'success' | 'warning' | 'error' | 'info' | 'default' {
    switch (status) {
      case 'Confirmed': return 'success';
      case 'Pending': return 'warning';
      case 'Cancelled': return 'error';
      case 'Completed': return 'info';
      default: return 'default';
    }
  }
}
