import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Hotel, RoomCategory, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { RoomCategoryCardComponent } from '../components/hotel/room-category-card.component';
import { AmenityIconComponent } from '../components/hotel/amenity-icon.component';
import { ButtonComponent } from '../components/ui/button.component';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-hotel-detail-page',
  standalone: true,
  imports: [CommonModule, RoomCategoryCardComponent, AmenityIconComponent, ButtonComponent],
  template: `
    <ng-container *ngIf="isLoading">
      <div class="max-w-7xl mx-auto px-4 py-20 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-gold-500 mx-auto"></div>
      </div>
    </ng-container>

    <ng-container *ngIf="!isLoading && hotel">
      <div class="min-h-screen bg-slate-50 pb-20">
        <!-- Hero Gallery -->
        <div class="h-[400px] md:h-[500px] relative">
          <img 
            [src]="hotel.imageUrl || 'https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=1920&q=80'" 
            class="w-full h-full object-cover"
            [alt]="hotel.name"
          />
          <div class="absolute inset-0 bg-gradient-to-t from-navy-900/80 to-transparent"></div>
          <div class="absolute bottom-10 left-0 right-0 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-white">
            <div class="flex items-center space-x-2 mb-2">
              
            </div>
            <h1 class="text-4xl md:text-5xl font-bold mb-4">{{ hotel.name }}</h1>
            <div class="flex items-center text-gray-200">
              
              <span class="text-lg">{{ hotel.address }}, {{ hotel.city }}, {{ hotel.country }}</span>
            </div>
          </div>
        </div>

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 -mt-10 relative z-10">
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Main Info -->
            <div class="lg:col-span-2 space-y-8">
              <section class="bg-white p-8 rounded-2xl shadow-sm border border-slate-100">
                <h2 class="text-2xl font-bold text-navy-900 mb-6">About the Hotel</h2>
                <p class="text-navy-600 leading-relaxed mb-8">{{ hotel.description }}</p>
                
                <h3 class="text-lg font-bold text-navy-900 mb-4">Hotel Amenities</h3>
                <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
                  <div *ngFor="let amenity of hotel.amenities" class="flex items-center p-3 bg-slate-50 rounded-lg">
                    <app-amenity-icon [amenity]="amenity" class="h-5 w-5 text-gold-500 mr-3"></app-amenity-icon>
                    <span class="text-sm font-medium text-navy-700">{{ amenity }}</span>
                  </div>
                </div>
              </section>

              <section>
                <h2 class="text-2xl font-bold text-navy-900 mb-6">Available Room Categories</h2>
                <div class="space-y-6">
                  <ng-container *ngIf="categories.length > 0; else noRoomsTpl">
                    <app-room-category-card 
                      *ngFor="let category of categories"
                      [category]="category" 
                      (onBook)="bookRoom($event)"
                    ></app-room-category-card>
                  </ng-container>
                  <ng-template #noRoomsTpl>
                    <div class="p-12 text-center bg-white rounded-xl border border-dashed border-slate-300">
                      <p class="text-navy-500">No rooms available for the selected dates.</p>
                    </div>
                  </ng-template>
                </div>
              </section>
            </div>

            <!-- Sidebar Info -->
            <div class="space-y-8">
              <section class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
                <h3 class="text-xl font-bold text-navy-900 mb-6">Why book with us?</h3>
                <ul class="space-y-4">
                  <li *ngFor="let item of reasons" class="flex items-start">
                    
                    <span class="text-navy-600 text-sm">{{ item }}</span>
                  </li>
                </ul>
              </section>
              
              <div class="bg-navy-900 p-8 rounded-2xl text-white">
                <h3 class="text-xl font-bold mb-4">Need help?</h3>
                <p class="text-navy-300 text-sm mb-6">Our travel experts are available 24/7 to help you with your booking.</p>
                <app-button variant="outline" class="w-full border-white text-white hover:bg-white hover:text-navy-900">Contact Support</app-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </ng-container>

    <ng-container *ngIf="!isLoading && !hotel">
      <div class="text-center py-20">Hotel not found</div>
    </ng-container>
  `
})
export class HotelDetailPageComponent implements OnInit {
  hotelId: string | null = null;
  hotel: Hotel | null = null;
  categories: RoomCategory[] = [];
  isLoading = true;

  reasons = [
    'Best price guarantee',
    'Earn 100 loyalty points per booking',
    'Free cancellation up to 24 hours before check-in',
    '24/7 dedicated customer support'
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.hotelId = params.get('id');
      if (this.hotelId) {
        this.fetchData();
      }
    });
  }

  fetchData() {
    this.isLoading = true;
    forkJoin({
      hotel: this.http.get<ApiResponse<Hotel>>(`${environment.apiUrl}/hotels/${this.hotelId}`),
      categories: this.http.get<ApiResponse<RoomCategory[]>>(`${environment.apiUrl}/rooms/hotel/${this.hotelId}/categories`)
    }).subscribe({
      next: (res) => {
        this.hotel = res.hotel.data;
        this.categories = res.categories.data || [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  bookRoom(categoryId: string) {
    this.router.navigate([`/hotels/${this.hotelId}/book/${categoryId}`]);
  }
}
