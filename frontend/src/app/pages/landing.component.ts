import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonComponent } from '../components/ui/button.component';
import { InputComponent } from '../components/ui/input.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent],
  template: `
    <div class="flex flex-col min-h-screen">
      <!-- Hero Section -->
      <section class="relative h-[600px] flex items-center justify-center text-white">
        <div 
          class="absolute inset-0 bg-cover bg-center z-0" 
          style="background-image: url('https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80')"
        >
          <div class="absolute inset-0 bg-navy-900/60"></div>
        </div>

        <div class="relative z-10 max-w-4xl w-full px-4 text-center">
          <h1 class="text-5xl md:text-6xl font-bold mb-6 tracking-tight">Experience Luxury Like Never Before</h1>
          <p class="text-xl mb-12 text-gray-200">Book your perfect stay at the world's most exclusive hotels.</p>
          
          <form (ngSubmit)="handleSearch()" class="bg-white p-4 rounded-xl shadow-2xl flex flex-col md:flex-row gap-4 items-end">
            <div class="flex-1 w-full text-left">
              <label class="text-navy-900 text-sm font-semibold mb-1 block">City</label>
              <app-input 
                placeholder="Where are you going?" 
                [(ngModel)]="searchParams.city"
                name="city"
                class="text-navy-900"
              ></app-input>
            </div>
            <div class="flex-1 w-full text-left">
              <label class="text-navy-900 text-sm font-semibold mb-1 block">Check-in</label>
              <app-input 
                type="date" 
                [(ngModel)]="searchParams.checkIn"
                name="checkIn"
                class="text-navy-900"
              ></app-input>
            </div>
            <div class="flex-1 w-full text-left">
              <label class="text-navy-900 text-sm font-semibold mb-1 block">Check-out</label>
              <app-input 
                type="date" 
                [(ngModel)]="searchParams.checkOut"
                name="checkOut"
                class="text-navy-900"
              ></app-input>
            </div>
            <div class="w-full md:w-32 text-left">
              <label class="text-navy-900 text-sm font-semibold mb-1 block">Guests</label>
              <app-input 
                type="number" 
                [(ngModel)]="searchParams.guests"
                name="guests"
                class="text-navy-900"
              ></app-input>
            </div>
            <app-button type="submit" class="w-full md:w-auto h-10 px-8">
               Search
            </app-button>
          </form>
        </div>
      </section>

      <!-- Features Section -->
      <section class="py-20 bg-white">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 class="text-3xl font-bold text-navy-900 mb-12">Why Choose LuxeStay?</h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-12">
            <div *ngFor="let feature of features" class="p-8 rounded-2xl bg-slate-50 border border-slate-100 transition-transform hover:-translate-y-2 group">
              <div class="text-4xl mb-4 grayscale group-hover:grayscale-0 transition-all">{{ feature.icon }}</div>
              <h3 class="text-xl font-bold mb-2 text-navy-900">{{ feature.title }}</h3>
              <p class="text-navy-600 leading-relaxed">{{ feature.desc }}</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  `
})
export class LandingPageComponent {
  searchParams = {
    city: '',
    checkIn: '',
    checkOut: '',
    guests: '1',
  };

  features = [
    { title: 'Best Prices', desc: 'Guaranteed lowest rates on luxury hotels.', icon: '₹' },
    { title: 'Wide Selection', desc: 'Over 5,000 premium hotels across India.', icon: '🏨' },
    { title: 'Loyalty Rewards', desc: 'Earn points on every booking and save more.', icon: '⭐' }
  ];

  constructor(private router: Router) {}

  handleSearch() {
    this.router.navigate(['/hotels'], { queryParams: this.searchParams });
  }
}
