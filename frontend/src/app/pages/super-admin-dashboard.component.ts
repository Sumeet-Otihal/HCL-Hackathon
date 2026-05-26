import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { DashboardStats, ApiResponse, User, Hotel } from '../models';
import { environment } from '../../environments/environment';
import { ButtonComponent } from '../components/ui/button.component';

@Component({
  selector: 'app-super-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonComponent],
  template: `
    <div class="max-w-7xl mx-auto px-4 py-12">
      <h1 class="text-3xl font-bold text-navy-900 mb-8">System Administration</h1>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="h-12 w-12 rounded-xl flex items-center justify-center mb-4 bg-blue-50 text-blue-600">
            
          </div>
          <p class="text-sm font-medium text-navy-500 mb-1">Total Bookings</p>
          <p class="text-2xl font-bold text-navy-900">{{ stats?.totalBookings || 0 }}</p>
        </div>

        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="h-12 w-12 rounded-xl flex items-center justify-center mb-4 bg-green-50 text-green-600">
            
          </div>
          <p class="text-sm font-medium text-navy-500 mb-1">Total Revenue</p>
          <p class="text-2xl font-bold text-navy-900">₹{{ (stats?.totalRevenue || 0) | number:'1.2-2' }}</p>
        </div>

        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="h-12 w-12 rounded-xl flex items-center justify-center mb-4 bg-gold-50 text-gold-600">
            
          </div>
          <p class="text-sm font-medium text-navy-500 mb-1">Active Hotels</p>
          <p class="text-2xl font-bold text-navy-900">{{ stats?.activeHotels || 0 }}</p>
        </div>

        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
          <div class="h-12 w-12 rounded-xl flex items-center justify-center mb-4 bg-navy-50 text-navy-600">
            
          </div>
          <p class="text-sm font-medium text-navy-500 mb-1">Total Users</p>
          <p class="text-2xl font-bold text-navy-900">{{ stats?.activeUsers || 0 }}</p>
        </div>
      </div>

      <div class="flex space-x-8 mb-8 border-b border-slate-200">
        <button 
          *ngFor="let tab of ['stats', 'hotels', 'users']"
          (click)="activeTab = tab"
          [ngClass]="{
            'pb-4 text-sm font-bold uppercase tracking-wider transition-colors border-b-2': true,
            'border-gold-500 text-gold-500': activeTab === tab,
            'border-transparent text-navy-500 hover:text-navy-900': activeTab !== tab
          }"
        >
          {{ tab }}
        </button>
      </div>

      <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
        <ng-container *ngIf="activeTab === 'hotels'">
          <div class="p-6">
            <div class="flex justify-between items-center mb-6">
              <h3 class="text-xl font-bold">Hotels Directory</h3>
              <app-button size="sm">
                 Add Hotel
              </app-button>
            </div>
            <table class="w-full text-left">
              <thead>
                <tr class="border-b border-slate-100">
                  <th class="py-4 font-bold text-navy-900">Hotel Name</th>
                  <th class="py-4 font-bold text-navy-900">City</th>
                  <th class="py-4 font-bold text-navy-900">Rating</th>
                  <th class="py-4 font-bold text-navy-900 text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let h of hotels" class="border-b border-slate-50 last:border-0 hover:bg-slate-50">
                  <td class="py-4 font-medium">{{ h.name }}</td>
                  <td class="py-4 text-navy-600">{{ h.city }}</td>
                  <td class="py-4 text-gold-500">{{ '★'.repeat(h.starRating) }}</td>
                  <td class="py-4 text-right">
                    <app-button variant="ghost" size="sm" class="text-navy-400">Edit</app-button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </ng-container>

        <ng-container *ngIf="activeTab === 'users'">
          <div class="p-6">
            <h3 class="text-xl font-bold mb-6">User Accounts</h3>
            <table class="w-full text-left">
              <thead>
                <tr class="border-b border-slate-100">
                  <th class="py-4 font-bold text-navy-900">User</th>
                  <th class="py-4 font-bold text-navy-900">Role</th>
                  <th class="py-4 font-bold text-navy-900">Points</th>
                  <th class="py-4 font-bold text-navy-900 text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let u of users" class="border-b border-slate-50 last:border-0 hover:bg-slate-50">
                  <td class="py-4">
                    <p class="font-medium">{{ u.firstName }} {{ u.lastName }}</p>
                    <p class="text-xs text-navy-500">{{ u.email }}</p>
                  </td>
                  <td class="py-4">
                    <span [ngClass]="{
                      'text-xs px-2 py-1 rounded-full font-bold uppercase tracking-wider': true,
                      'bg-red-100 text-red-700': u.role === 'SuperAdmin',
                      'bg-blue-100 text-blue-700': u.role === 'HotelAdmin',
                      'bg-slate-100 text-slate-700': u.role === 'User'
                    }">
                      {{ u.role }}
                    </span>
                  </td>
                  <td class="py-4 text-navy-600">{{ u.loyaltyPoints }}</td>
                  <td class="py-4 text-right">
                    <app-button variant="ghost" size="sm" class="text-navy-400">Manage</app-button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </ng-container>

        <ng-container *ngIf="activeTab === 'stats'">
          <div class="p-20 text-center text-navy-400">
            
            <p>Select a metric to see detailed analytics.</p>
          </div>
        </ng-container>
      </div>
    </div>
  `
})
export class SuperAdminDashboardComponent implements OnInit {
  activeTab = 'stats';
  stats: DashboardStats | null = null;
  hotels: Hotel[] = [];
  users: User[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    this.http.get<ApiResponse<DashboardStats>>(`${environment.apiUrl}/admin/stats`)
      .subscribe(res => this.stats = res.data);
      
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/hotels`)
      .subscribe(res => this.hotels = res.data.items || []);
      
    this.http.get<ApiResponse<User[]>>(`${environment.apiUrl}/admin/users`)
      .subscribe(res => this.users = res.data || []);
  }
}
