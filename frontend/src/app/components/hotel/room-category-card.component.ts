import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoomCategory } from '../../models';
import { ButtonComponent } from '../ui/button.component';
import { AmenityIconComponent } from './amenity-icon.component';

@Component({
  selector: 'app-room-category-card',
  standalone: true,
  imports: [CommonModule, ButtonComponent, AmenityIconComponent],
  template: `
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden flex flex-col md:flex-row">
      <div class="md:w-1/3 h-48 md:h-auto">
        <img 
          [src]="category.imageUrls?.[0] || 'https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=800&q=80'" 
          [alt]="category.name"
          class="w-full h-full object-cover"
        />
      </div>
      
      <div class="p-6 md:w-2/3 flex flex-col justify-between">
        <div>
          <div class="flex justify-between items-start mb-2">
            <h3 class="text-xl font-bold text-navy-900">{{ category.name }}</h3>
            <p class="text-2xl font-bold text-gold-600">₹{{ category.pricePerNight | number:'1.2-2' }}<span class="text-sm text-navy-500 font-normal"> / night</span></p>
          </div>
          
          <p class="text-navy-600 text-sm mb-4 line-clamp-2">{{ category.description }}</p>
          
          <div class="flex items-center space-x-6 text-sm text-navy-500 mb-6">
            <div class="flex items-center">
              
              <span>Up to {{ category.maxOccupancy }} Guests</span>
            </div>
            <div class="flex flex-wrap gap-4">
              <div *ngFor="let amenity of category.amenities | slice:0:4" class="flex items-center">
                <app-amenity-icon [amenity]="amenity" class="h-4 w-4 mr-2"></app-amenity-icon>
                <span>{{ amenity }}</span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="flex justify-end">
          <app-button (onClick)="onBook.emit(category.id)">Book Now</app-button>
        </div>
      </div>
    </div>
  `
})
export class RoomCategoryCardComponent {
  @Input() category!: RoomCategory;
  @Output() onBook = new EventEmitter<string>();
  
  }
