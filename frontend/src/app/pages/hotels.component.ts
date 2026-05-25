import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Hotel, ApiResponse } from '../models';
import { environment } from '../../environments/environment';
import { HotelCardComponent } from '../components/hotel/hotel-card.component';
import { InputComponent } from '../components/ui/input.component';
import { ButtonComponent } from '../components/ui/button.component';

@Component({
  selector: 'app-hotels-page',
  standalone: true,
  imports: [CommonModule, FormsModule, HotelCardComponent, InputComponent, ButtonComponent],
  template: `
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="flex flex-col md:flex-row gap-8">
        <!-- Sidebar Filters -->
        <aside [ngClass]="{
            'md:w-64 space-y-8 bg-white p-6 rounded-xl border border-slate-200 h-fit sticky top-24 transition-transform md:translate-x-0': true,
            'fixed inset-0 z-40 translate-x-0': isFilterOpen,
            'hidden md:block': !isFilterOpen
          }">
          <div class="flex items-center justify-between md:hidden mb-6">
            <h2 class="text-xl font-bold">Filters</h2>
            <button (click)="isFilterOpen = false"></button>
          </div>

          <div>
            <h3 class="text-sm font-semibold text-navy-900 mb-4 uppercase tracking-wider">City</h3>
            <app-input 
              placeholder="Search city..." 
              [value]="filters.city"
              (ngModelChange)="updateFilter('city', $event)"
              [(ngModel)]="filters.city"
            ></app-input>
          </div>

          <div>
            <h3 class="text-sm font-semibold text-navy-900 mb-4 uppercase tracking-wider">Star Rating</h3>
            <div class="space-y-2">
              <label *ngFor="let star of [5, 4, 3, 2, 1]" class="flex items-center space-x-3 cursor-pointer group">
                <input 
                  type="checkbox" 
                  [checked]="hasStar(star)"
                  (change)="toggleStar(star, $event)"
                  class="h-4 w-4 text-gold-500 rounded border-slate-300 focus:ring-gold-500" 
                />
                <span class="text-navy-600 group-hover:text-navy-900 transition-colors">{{ star }} Stars</span>
              </label>
            </div>
          </div>

          <div>
            <h3 class="text-sm font-semibold text-navy-900 mb-4 uppercase tracking-wider">Price Range (Per Night)</h3>
            <div class="space-y-4">
              <app-input 
                type="number" 
                placeholder="Min" 
                [value]="filters.minPrice"
                (ngModelChange)="updateFilter('minPrice', $event)"
                [(ngModel)]="filters.minPrice"
              ></app-input>
              <app-input 
                type="number" 
                placeholder="Max" 
                [value]="filters.maxPrice"
                (ngModelChange)="updateFilter('maxPrice', $event)"
                [(ngModel)]="filters.maxPrice"
              ></app-input>
            </div>
          </div>

          <app-button variant="outline" class="w-full block" (onClick)="clearFilters()">Clear All Filters</app-button>
        </aside>

        <!-- Main Content -->
        <main class="flex-1">
          <div class="flex items-center justify-between mb-8">
            <h1 class="text-2xl font-bold text-navy-900">
              {{ isLoading ? 'Searching Hotels...' : (hotels.length + ' Hotels Found') }}
            </h1>
            <app-button 
              variant="outline" 
              class="md:hidden"
              (onClick)="isFilterOpen = true"
            >
               Filters
            </app-button>
          </div>

          <ng-container *ngIf="isLoading; else contentTpl">
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              <div *ngFor="let i of [1, 2, 3, 4, 5, 6]" class="bg-slate-200 animate-pulse rounded-xl h-[400px]"></div>
            </div>
          </ng-container>

          <ng-template #contentTpl>
            <ng-container *ngIf="isError; else dataTpl">
              <div class="text-center py-20">
                <p class="text-red-500 mb-4">Failed to load hotels. Please try again.</p>
                <app-button (onClick)="fetchHotels()">Retry</app-button>
              </div>
            </ng-container>

            <ng-template #dataTpl>
              <ng-container *ngIf="hotels.length === 0; else listTpl">
                <div class="text-center py-20 bg-white rounded-xl border border-dashed border-slate-300">
                  
                  <h3 class="text-xl font-bold text-navy-900">No hotels found</h3>
                  <p class="text-navy-500">Try adjusting your filters to find more results.</p>
                </div>
              </ng-container>

              <ng-template #listTpl>
                <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                  <app-hotel-card *ngFor="let hotel of hotels" [hotel]="hotel"></app-hotel-card>
                </div>

                <!-- Pagination -->
                <div class="mt-12 flex justify-center items-center space-x-4">
                  <app-button 
                    variant="outline" 
                    [disabled]="page === 1"
                    (onClick)="updatePage(page - 1)"
                  >
                     Previous
                  </app-button>
                  <span class="text-sm font-medium">Page {{ page }}</span>
                  <app-button 
                    variant="outline"
                    [disabled]="hotels.length < 9"
                    (onClick)="updatePage(page + 1)"
                  >
                    Next 
                  </app-button>
                </div>
              </ng-template>
            </ng-template>
          </ng-template>
        </main>
      </div>
    </div>
  `
})
export class HotelsPageComponent implements OnInit {
  isFilterOpen = false;
  isLoading = false;
  isError = false;
  hotels: Hotel[] = [];

  filters: any = {
    city: '',
    starRating: '',
    minPrice: '',
    maxPrice: ''
  };
  page = 1;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.filters.city = params['city'] || '';
      this.filters.starRating = params['starRating'] || '';
      this.filters.minPrice = params['minPrice'] || '';
      this.filters.maxPrice = params['maxPrice'] || '';
      this.page = parseInt(params['page'] || '1', 10);
      
      this.fetchHotels();
    });
  }

  fetchHotels() {
    this.isLoading = true;
    this.isError = false;
    
    const params: any = { ...this.filters, page: this.page.toString(), pageSize: '9' };
    
    // clean empty params
    Object.keys(params).forEach(key => {
      if (!params[key]) delete params[key];
    });

    this.http.get<ApiResponse<Hotel[]>>(`${environment.apiUrl}/hotels/search`, { params })
      .subscribe({
        next: (res) => {
          this.hotels = res.data;
          this.isLoading = false;
        },
        error: () => {
          this.isError = true;
          this.isLoading = false;
        }
      });
  }

  updateFilter(key: string, value: any) {
    this.filters[key] = value;
    this.page = 1; // reset page on filter change
    this.applyFilters();
  }

  updatePage(newPage: number) {
    this.page = newPage;
    this.applyFilters();
  }

  hasStar(star: number): boolean {
    return this.filters.starRating.split(',').includes(star.toString());
  }

  toggleStar(star: number, event: any) {
    const current = this.filters.starRating.split(',').filter(Boolean);
    const checked = event.target.checked;
    
    let next;
    if (checked) {
      next = [...current, star.toString()];
    } else {
      next = current.filter((s: string) => s !== star.toString());
    }
    
    this.filters.starRating = next.join(',');
    this.page = 1;
    this.applyFilters();
  }

  applyFilters() {
    const queryParams: any = { ...this.filters, page: this.page > 1 ? this.page : null };
    Object.keys(queryParams).forEach(key => {
      if (!queryParams[key]) delete queryParams[key];
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      replaceUrl: true
    });
  }

  clearFilters() {
    this.filters = { city: '', starRating: '', minPrice: '', maxPrice: '' };
    this.page = 1;
    this.router.navigate([], { relativeTo: this.route });
  }
}
