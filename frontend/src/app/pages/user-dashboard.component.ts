import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
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
      <div *ngIf="showSuccess" class="mb-8 p-4 bg-green-50 border border-green-200 text-green-700 rounded-xl flex items-center justify-between">
        <div class="flex items-center">
          
          <span class="font-bold">Booking Confirmed!</span>
          <span class="ml-2">Your reservation has been successfully placed.</span>
        </div>
        <button (click)="showSuccess = false" class="text-green-500 hover:text-green-700 font-bold">✕</button>
      </div>

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
                <img [src]="booking.hotel?.imageUrls?.[0]" class="w-full h-full object-cover" [alt]="booking.hotel?.name" />
              </div>
              <div class="p-6 flex-1 flex flex-col md:flex-row md:items-center justify-between gap-6">
                <div class="space-y-2">
                  <div class="flex items-center space-x-2">
                    <h3 class="font-bold text-lg text-navy-900">{{ booking.hotelName }}</h3>
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
                  <p class="text-xl font-bold text-navy-900">₹{{ booking.finalAmount | number:'1.2-2' }}</p>
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
                    <app-button 
                      variant="ghost" 
                      size="sm" 
                      class="block"
                      (onClick)="viewDetails(booking)"
                    >
                      View Details
                    </app-button>
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

      <!-- Booking Details Modal -->
      <div *ngIf="selectedBooking" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-navy-900/60 backdrop-blur-sm">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden animate-in fade-in zoom-in duration-200">
          <div class="bg-navy-900 p-6 text-white flex justify-between items-center">
            <div>
              <h2 class="text-xl font-bold">Booking Details</h2>
              <p class="text-navy-300 text-sm">Res #{{ selectedBooking.reservationNumber }}</p>
            </div>
            <button (click)="closeModal()" class="text-navy-300 hover:text-white transition-colors">
              ✕
            </button>
          </div>
          
          <div class="p-8 space-y-6">
            <div class="flex items-center space-x-4 pb-6 border-b border-slate-100">
              <img [src]="selectedBooking.hotel?.imageUrls?.[0]" class="w-20 h-20 rounded-xl object-cover" />
              <div>
                <h3 class="text-xl font-bold text-navy-900">{{ selectedBooking.hotelName }}</h3>
                <p class="text-navy-500">{{ selectedBooking.roomCategory }}</p>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-8">
              <div>
                <p class="text-xs text-navy-400 uppercase tracking-widest font-bold mb-1">Check-in</p>
                <p class="font-bold text-navy-900">{{ selectedBooking.checkInDate | date:'fullDate' }}</p>
              </div>
              <div>
                <p class="text-xs text-navy-400 uppercase tracking-widest font-bold mb-1">Check-out</p>
                <p class="font-bold text-navy-900">{{ selectedBooking.checkOutDate | date:'fullDate' }}</p>
              </div>
              <div>
                <p class="text-xs text-navy-400 uppercase tracking-widest font-bold mb-1">Status</p>
                <app-badge [variant]="getVariant(selectedBooking.status)">{{ selectedBooking.status }}</app-badge>
              </div>
              <div>
                <p class="text-xs text-navy-400 uppercase tracking-widest font-bold mb-1">Nights</p>
                <p class="font-bold text-navy-900">{{ selectedBooking.totalNights }} Nights</p>
              </div>
            </div>

            <div class="bg-slate-50 p-6 rounded-2xl space-y-3">
              <div class="flex justify-between text-navy-600">
                <span>Subtotal</span>
                <span>₹{{ selectedBooking.totalAmount | number:'1.2-2' }}</span>
              </div>
              <div *ngIf="selectedBooking.discountAmount > 0" class="flex justify-between text-green-600 font-medium">
                <span>Discount</span>
                <span>- ₹{{ selectedBooking.discountAmount | number:'1.2-2' }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold text-navy-900 pt-3 border-t border-slate-200">
                <span>Total Paid</span>
                <span class="text-gold-600">₹{{ selectedBooking.finalAmount | number:'1.2-2' }}</span>
              </div>
            </div>

            <app-button variant="outline" class="w-full h-12" (onClick)="closeModal()">
              Close Details
            </app-button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class UserDashboardComponent implements OnInit {
  activeTab: 'bookings' | 'loyalty' = 'bookings';
  bookings: Booking[] = [];
  isLoading = true;
  cancelingId: string | null = null;
  showSuccess = false;
  selectedBooking: Booking | null = null;

  loyaltyTransactions = [
    { desc: 'Booking at Taj Palace', points: '+500', date: '20 May 2026', type: 'earn' },
    { desc: 'Redeemed on Holiday Inn', points: '-200', date: '15 May 2026', type: 'redeem' },
    { desc: 'Signup Bonus', points: '+950', date: '10 May 2026', type: 'earn' },
  ];

  constructor(private http: HttpClient, private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['bookingSuccess']) {
        this.showSuccess = true;
      }
    });
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

  viewDetails(booking: Booking) {
    this.selectedBooking = booking;
  }

  closeModal() {
    this.selectedBooking = null;
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

