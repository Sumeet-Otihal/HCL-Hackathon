import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/layout/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    <div class="flex flex-col min-h-screen bg-slate-50">
      <app-navbar></app-navbar>
      <main class="flex-grow">
        <router-outlet></router-outlet>
      </main>
      <footer class="bg-navy-900 text-white py-12 border-t border-navy-800">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-12">
            <div class="col-span-1 md:col-span-2">
              <h2 class="text-2xl font-bold text-gold-500 mb-4 tracking-tight">LuxeStay</h2>
              <p class="max-w-sm text-navy-200 leading-relaxed">
                LuxeStay is the world's leading luxury hotel booking platform, 
                connecting you with the most exclusive stays across the globe.
              </p>
            </div>
            <div>
              <h3 class="text-white font-bold mb-4 uppercase text-sm tracking-widest">Quick Links</h3>
              <ul class="space-y-2 text-sm text-navy-300">
                <li><a href="/hotels" class="hover:text-gold-500 transition-colors">Find Hotels</a></li>
                <li><a href="/register" class="hover:text-gold-500 transition-colors">Join Loyalty Program</a></li>
                <li><a href="/support" class="hover:text-gold-500 transition-colors">Customer Support</a></li>
              </ul>
            </div>
          </div>
          <div class="mt-12 pt-8 border-t border-navy-800 text-center text-xs text-navy-400">
            © 2026 LuxeStay Hospitality Group. All rights reserved.
          </div>
        </div>
      </footer>
    </div>
  `
})
export class AppComponent {
  title = 'frontend';
}
