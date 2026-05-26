import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Booking, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { ButtonComponent } from '../components/ui/button.component';

declare global {
  interface Window {
    Razorpay: any;
  }
}

@Component({
  selector: 'app-payment-page',
  standalone: true,
  imports: [CommonModule, ButtonComponent],
  template: `
    <ng-container *ngIf="isLoading">
      <div class="p-20 text-center">Loading payment details...</div>
    </ng-container>

    <ng-container *ngIf="!isLoading && !booking">
      <div class="p-20 text-center">Booking not found</div>
    </ng-container>

    <ng-container *ngIf="!isLoading && booking">
      <div class="max-w-2xl mx-auto px-4 py-12">
        <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
          <div class="bg-navy-900 p-8 text-white text-center">
            
            <h1 class="text-2xl font-bold">Secure Payment</h1>
            <p class="text-navy-300">Complete your reservation for {{ booking.hotelName }}</p>
          </div>
          
          <div class="p-8 space-y-8">
            <div class="flex justify-between items-center pb-6 border-b border-slate-100">
              <div>
                <p class="text-sm text-navy-500 uppercase tracking-wider font-semibold">Amount to Pay</p>
                <p class="text-3xl font-bold text-navy-900">₹{{ booking.finalAmount | number:'1.2-2' }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm text-navy-500 uppercase tracking-wider font-semibold">Reservation #</p>
                <p class="text-lg font-mono font-bold text-navy-700">{{ booking.reservationNumber }}</p>
              </div>
            </div>

            <div class="bg-slate-50 p-4 rounded-xl flex items-start space-x-3">
              
              <p class="text-sm text-navy-600">
                By clicking "Pay Now", you agree to our terms and conditions and cancellation policy. 
                The transaction is secured with 256-bit encryption.
              </p>
            </div>

            <app-button 
              class="w-full h-14 text-lg block" 
              (onClick)="createOrder()"
              [isLoading]="isProcessing"
            >
              Pay Now ₹{{ booking.finalAmount | number:'1.2-2' }}
            </app-button>

            <div class="flex justify-center items-center space-x-4 grayscale opacity-50">
              <img src="https://upload.wikimedia.org/wikipedia/commons/8/89/Razorpay_logo.svg" class="h-4" alt="Razorpay" />
              <img src="https://upload.wikimedia.org/wikipedia/commons/5/5e/Visa_Inc._logo.svg" class="h-4" alt="Visa" />
              <img src="https://upload.wikimedia.org/wikipedia/commons/2/2a/Mastercard-logo.svg" class="h-4" alt="Mastercard" />
            </div>
          </div>
        </div>
      </div>
    </ng-container>
  `
})
export class PaymentPageComponent implements OnInit {
  bookingId: string | null = null;
  booking: any | null = null;
  isLoading = true;
  isProcessing = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.bookingId = params.get('bookingId');
      if (this.bookingId) {
        this.fetchBooking();
      }
    });
  }

  fetchBooking() {
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/bookings/${this.bookingId}`)
      .subscribe({
        next: (res) => {
          this.booking = res.data;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        }
      });
  }

  createOrder() {
    this.isProcessing = true;
    // Simulate payment processing delay
    setTimeout(() => {
      this.verifyPayment({
        bookingId: Number(this.bookingId),
        razorpayPaymentId: 'pay_dummy_' + Math.random().toString(36).substring(7),
        razorpayOrderId: 'order_dummy_' + Math.random().toString(36).substring(7),
        razorpaySignature: 'dummy_sig'
      });
    }, 1500);
  }

  verifyPayment(payload: any) {
    this.http.post<ApiResponse<any>>(`${environment.apiUrl}/payments/verify`, payload)
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.router.navigate(['/user-dashboard'], { queryParams: { bookingSuccess: 'true' } });
          }
          this.isProcessing = false;
        },
        error: (err) => {
          console.error('Payment verification failed:', err);
          this.isProcessing = false;
        }
      });
  }
}
