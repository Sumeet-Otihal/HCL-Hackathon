import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Hotel } from '../../models';
import { ButtonComponent } from '../ui/button.component';
import { BadgeComponent } from '../ui/badge.component';
import { AmenityIconComponent } from './amenity-icon.component';

@Component({
  selector: 'app-hotel-card',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonComponent, BadgeComponent, AmenityIconComponent],
  template: `
    <div class="bg-white rounded-xl shadow-md overflow-hidden border border-slate-200 hover:shadow-lg transition-shadow">
      <div class="relative h-48 sm:h-64">
        <img 
          [src]="hotel.imageUrl || 'https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80'" 
          [alt]="hotel.name"
          class="w-full h-full object-cover"
        />
        <div class="absolute top-4 right-4 bg-white px-2 py-1 rounded-md flex items-center shadow-sm">
          
          <span class="ml-1 text-sm font-bold text-navy-900">{{ hotel.starRating }}</span>
        </div>
      </div>
      
      <div class="p-6">
        <div class="flex justify-between items-start mb-2">
          <h3 class="text-xl font-bold text-navy-900 truncate">{{ hotel.name }}</h3>
        </div>
        
        <div class="flex items-center text-navy-500 text-sm mb-4">
          
          <span>{{ hotel.city }}, {{ hotel.country }}</span>
        </div>

        <div class="flex flex-wrap gap-2 mb-6">
          <app-badge *ngFor="let amenity of hotel.amenities | slice:0:3" variant="default" class="flex items-center">
            <app-amenity-icon [amenity]="amenity" class="h-3 w-3 mr-1"></app-amenity-icon>
            {{ amenity }}
          </app-badge>
          <span *ngIf="hotel.amenities.length > 3" class="text-xs text-navy-400">
            +{{ hotel.amenities.length - 3 }} more
          </span>
        </div>

        <div class="flex items-center justify-between border-t border-slate-100 pt-4">
          <div>
            <p class="text-xs text-navy-500">Starting from</p>
            <p class="text-lg font-bold text-gold-600">₹{{ priceFrom || '2,499' | number }}</p>
          </div>
          <a [routerLink]="['/hotels', hotel.id]">
            <app-button size="sm">View Details</app-button>
          </a>
        </div>
      </div>
    </div>
  `
})
export class HotelCardComponent {
  @Input() hotel!: Hotel;
  @Input() priceFrom?: number;

  }
