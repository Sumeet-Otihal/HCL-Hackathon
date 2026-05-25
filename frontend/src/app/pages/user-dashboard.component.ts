import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Booking, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { ButtonComponent } from '../components/ui/button.component';
import { BadgeComponent } from '../components/ui/badge.component';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonComponent, BadgeComponent],
  template: `
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 class="text-3xl font-bold text-navy-900 mb-8">My Dashboard</h1>

      <div class="flex space-x-8 mb-8 border-b border-slate-200">
        <button 
          (click)="activeTab = 'bookings'"
          [ngClass]="{
            'pb-4 text-sm font-bold uppercase tracking-wider transition-colors border-b-2': true,
            'border-gold-500 text-gold-500': activeTab === 'bookings',
            'border-transparent text-navy-500 hover:text-navy-900': activeTab !== 'bookings'
          }"
        >
          My Bookings
        </button>
        <button 
          (click)="activeTab = 'loyalty'"
          [ngClass]="{
            'pb-4 text-sm font-bold uppercase tracking-wider transition-colors border-b-2': true,
            'border-gold-500 text-gold-500': activeTab === 'loyalty',
            'border-transparent text-navy-500 hover:text-navy-900': activeTab !== 'loyalty'
          }"
        >
          Loyalty Points
        </button>
      </div>

      <ng-container *ngIf="activeTab === 'bookings'">
        <div class="space-y-6">
          <ng-container *ngIf="isLoading">
            <div *ngFor="let i of [1, 2, 3]" class="h-40 bg-slate-100 animate-pulse rounded-xl"></div>
          </ng-container>
          
          <ng-container *ngIf="!isLoading && bookings.length === 0">
            <div class="text-center py-20 bg-white rounded-2xl border border-dashed border-slate-200">
              
              <p class="text-navy-500">You haven't made any bookings yet.</p>
              <a href="/hotels" class="mt-4 inline-block"><app-button variant="outline">Explore Hotels</app-button></a>
            </div>
          </ng-container>

          <ng-container *ngIf="!isLoading && bookings.length > 0">
            <div *ngFor="let booking of bookings" class="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden flex flex-col md:flex-row">
              <div class="md:w-1/4 h-32 md:h-auto">
                <img [src]="booking.hotel?.imageUrl" class="w-full h-full object-cover" [alt]="booking.hotel?.name" />
              </div>
              <div class="p-6 flex-1 flex flex-col md:flex-row md:items-center justify-between gap-6">
                <div class="space-y-2">
                  <div class="flex items-center space-x-2">
                    <h3 class="font-bold text-lg text-navy-900">{{ booking.hotel?.name }}</h3>
                    <app-badge [variant]="getVariant(booking.status)">{{ booking.status }}</app-badge>
                  </div>
                  <div class="flex items-center text-sm text-navy-500">
                    
                    <span>{{ booking.checkInDate | date:'dd MMM yyyy' }} - {{ booking.checkOutDate | date:'dd MMM yyyy' }}</span>
                  </div>
                  <div class="flex items-center text-sm text-navy-500">
                    
                    <span>{{ booking.hotel?.city }}</span>
                  </div>
                </div>

                <div class="text-right space-y-2">
                  <p class="text-xl font-bold text-navy-900">₹{{ booking.totalPrice | number:'1.2-2' }}</p>
                  <div class="flex flex-wrap gap-2 justify-end">
                    <app-button 
                      *ngIf="booking.status === 'Confirmed'"
                      variant="outline" 
                      size="sm" 
                      class="text-red-500 border-red-200 hover:bg-red-50 block"
                      (onClick)="cancelBooking(booking.id)"
                      [isLoading]="cancelingId === booking.id"
                    >
                      Cancel Booking
                    </app-button>
                    <app-button variant="ghost" size="sm" class="block">View Details </app-button>
                  </div>
                </div>
              </div>
            </div>
          </ng-container>
        </div>
      </ng-container>

      <ng-container *ngIf="activeTab === 'loyalty'">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div class="lg:col-span-1">
            <div class="bg-navy-900 p-8 rounded-2xl text-white text-center">
              <p class="text-navy-300 uppercase tracking-widest text-xs font-bold mb-2">Current Balance</p>
              <p class="text-5xl font-bold text-gold-500">1,250</p>
              <p class="mt-4 text-sm text-navy-400">≈ ₹1,250.00 worth of discounts</p>
            </div>
          </div>
          <div class="lg:col-span-2 bg-white rounded-2xl border border-slate-100 p-8">
            <h3 class="text-xl font-bold mb-6">Transaction History</h3>
            <div class="space-y-4">
              <div *ngFor="let t of loyaltyTransactions" class="flex justify-between items-center py-4 border-b border-slate-50 last:border-0">
                <div>
                  <p class="font-bold text-navy-900">{{ t.desc }}</p>
                  <p class="text-xs text-navy-500">{{ t.date }}</p>
                </div>
                <p [ngClass]="{'font-bold text-lg': true, 'text-green-600': t.type === 'earn', 'text-red-600': t.type === 'redeem'}">
                  {{ t.points }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </ng-container>
    </div>
  `
})
export class UserDashboardComponent implements OnInit {
  activeTab: 'bookings' | 'loyalty' = 'bookings';
  bookings: Booking[] = [];
  isLoading = true;
  cancelingId: string | null = null;

  loyaltyTransactions = [
    { desc: 'Booking at Taj Palace', points: '+500', date: '20 May 2026', type: 'earn' },
    { desc: 'Redeemed on Holiday Inn', points: '-200', date: '15 May 2026', type: 'redeem' },
    { desc: 'Signup Bonus', points: '+950', date: '10 May 2026', type: 'earn' },
  ];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchBookings();
  }

  fetchBookings() {
    this.isLoading = true;
    this.http.get<ApiResponse<Booking[]>>(`${environment.apiUrl}/bookings/my-bookings`)
      .subscribe({
        next: (res) => {
          this.bookings = res.data || [];
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        }
      });
  }

  cancelBooking(id: string) {
    this.cancelingId = id;
    this.http.post<ApiResponse<any>>(`${environment.apiUrl}/bookings/${id}/cancel`, {})
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.fetchBookings();
          }
          this.cancelingId = null;
        },
        error: () => {
          this.cancelingId = null;
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
