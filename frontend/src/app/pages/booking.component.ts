import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { differenceInDays, isBefore, addDays, format } from 'date-fns';
import { Hotel, RoomCategory, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { InputComponent } from '../components/ui/input.component';
import { ButtonComponent } from '../components/ui/button.component';
import { PriceBreakdownComponent } from '../components/booking/price-breakdown.component';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-booking-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputComponent, ButtonComponent, PriceBreakdownComponent],
  template: `
    <ng-container *ngIf="isLoading">
      <div class="p-20 text-center">Loading booking details...</div>
    </ng-container>

    <ng-container *ngIf="!isLoading && hotel && category">
      <div class="max-w-5xl mx-auto px-4 py-12">
        <h1 class="text-3xl font-bold text-navy-900 mb-8">Confirm your Booking</h1>
        
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-12">
          <div class="lg:col-span-2 space-y-8">
            <section class="bg-white p-8 rounded-2xl shadow-sm border border-slate-100">
              <h2 class="text-xl font-bold mb-6">Booking Details</h2>
              <form [formGroup]="bookingForm" (ngSubmit)="createBooking()" class="space-y-6">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <app-input 
                    label="Check-in Date" 
                    type="date" 
                    formControlName="checkInDate"
                    [error]="(bookingForm.get('checkInDate')?.touched && bookingForm.get('checkInDate')?.invalid) ? 'Check-in date is required' : null"
                  ></app-input>
                  <app-input 
                    label="Check-out Date" 
                    type="date" 
                    formControlName="checkOutDate"
                    [error]="bookingForm.errors?.['dateRange'] ? 'Check-out date must be after check-in date' : (bookingForm.get('checkOutDate')?.touched && bookingForm.get('checkOutDate')?.invalid ? 'Check-out date is required' : null)"
                  ></app-input>
                </div>
                
                <app-input 
                  label="Number of Guests" 
                  type="number" 
                  formControlName="guests"
                  [error]="(bookingForm.get('guests')?.touched && bookingForm.get('guests')?.invalid) ? 'Invalid number of guests' : null"
                ></app-input>

                <div class="flex gap-4 items-end">
                  <div class="flex-1">
                    <app-input 
                      label="Promo Code" 
                      placeholder="Enter code" 
                      formControlName="promoCode"
                    ></app-input>
                  </div>
                  <app-button 
                    type="button" 
                    variant="outline" 
                    (onClick)="applyPromo()"
                    [isLoading]="isApplyingPromo"
                  >
                    Apply
                  </app-button>
                </div>

                <div class="pt-8">
                  <app-button 
                    type="submit" 
                    class="w-full h-12 text-lg" 
                    [isLoading]="isSubmitting"
                    [disabled]="bookingForm.invalid"
                  >
                    Confirm and Pay
                  </app-button>
                </div>
              </form>
            </section>
          </div>

          <div class="space-y-6">
            <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
              <h3 class="text-lg font-bold mb-4">Reservation Summary</h3>
              <div class="flex space-x-4 mb-6">
                <img [src]="category.imageUrl" class="w-20 h-20 rounded-lg object-cover" />
                <div>
                  <p class="font-bold text-navy-900">{{ hotel.name }}</p>
                  <p class="text-sm text-navy-500">{{ category.name }}</p>
                </div>
              </div>
              
              <app-price-breakdown 
                [basePrice]="category.basePrice" 
                [nights]="nights" 
                [discount]="discount" 
                [promoCode]="appliedPromo"
              ></app-price-breakdown>
            </div>
          </div>
        </div>
      </div>
    </ng-container>
  `
})
export class BookingPageComponent implements OnInit {
  hotelId: string | null = null;
  categoryId: string | null = null;
  hotel: Hotel | null = null;
  category: RoomCategory | null = null;
  
  isLoading = true;
  isApplyingPromo = false;
  isSubmitting = false;

  bookingForm!: FormGroup;
  discount = 0;
  appliedPromo?: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.hotelId = params.get('id');
      this.categoryId = params.get('categoryId');
      if (this.hotelId && this.categoryId) {
        this.fetchData();
      }
    });

    const checkIn = new Date();
    const checkOut = addDays(checkIn, 1);

    this.bookingForm = this.fb.group({
      checkInDate: [format(checkIn, 'yyyy-MM-dd'), Validators.required],
      checkOutDate: [format(checkOut, 'yyyy-MM-dd'), Validators.required],
      guests: [1, [Validators.required, Validators.min(1)]],
      promoCode: ['']
    }, { validators: this.dateRangeValidator });
  }

  dateRangeValidator(group: FormGroup) {
    const start = group.get('checkInDate')?.value;
    const end = group.get('checkOutDate')?.value;
    if (start && end && !isBefore(new Date(start), new Date(end))) {
      return { dateRange: true };
    }
    return null;
  }

  get nights(): number {
    const checkIn = this.bookingForm.get('checkInDate')?.value;
    const checkOut = this.bookingForm.get('checkOutDate')?.value;
    if (checkIn && checkOut && isBefore(new Date(checkIn), new Date(checkOut))) {
      return Math.max(1, differenceInDays(new Date(checkOut), new Date(checkIn)));
    }
    return 1;
  }

  fetchData() {
    this.isLoading = true;
    forkJoin({
      hotel: this.http.get<ApiResponse<Hotel>>(`${environment.apiUrl}/hotels/${this.hotelId}`),
      category: this.http.get<ApiResponse<RoomCategory>>(`${environment.apiUrl}/rooms/categories/${this.categoryId}`)
    }).subscribe({
      next: (res) => {
        this.hotel = res.hotel.data;
        this.category = res.category.data;
        this.bookingForm.get('guests')?.setValidators([Validators.required, Validators.min(1), Validators.max(this.category!.maxOccupancy)]);
        this.bookingForm.get('guests')?.updateValueAndValidity();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  applyPromo() {
    const code = this.bookingForm.get('promoCode')?.value;
    if (!code) return;

    this.isApplyingPromo = true;
    this.http.post<ApiResponse<{ discountPercentage: number }>>(`${environment.apiUrl}/promotions/validate`, { code })
      .subscribe({
        next: (res) => {
          if (res.success && this.category) {
            this.discount = (this.category.basePrice * this.nights) * (res.data.discountPercentage / 100);
            this.appliedPromo = code;
          }
          this.isApplyingPromo = false;
        },
        error: () => {
          this.isApplyingPromo = false;
        }
      });
  }

  createBooking() {
    if (this.bookingForm.invalid || !this.category) return;
    
    this.isSubmitting = true;
    const data = this.bookingForm.value;
    const totalPrice = (this.category.basePrice * this.nights) - this.discount;

    this.http.post<ApiResponse<{ id: string }>>(`${environment.apiUrl}/bookings`, {
      hotelId: this.hotelId,
      categoryId: this.categoryId,
      checkInDate: data.checkInDate,
      checkOutDate: data.checkOutDate,
      guests: data.guests,
      promoCode: data.promoCode,
      totalPrice
    }).subscribe({
      next: (res) => {
        if (res.success) {
          this.router.navigate([`/payment/${res.data.id}`]);
        }
        this.isSubmitting = false;
      },
      error: () => {
        this.isSubmitting = false;
      }
    });
  }
}
