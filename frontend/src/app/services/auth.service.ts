import { Injectable, signal, computed } from '@angular/core';
import { User, AuthResponse, ApiResponse } from '../models';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSignal = signal<User | null>(null);
  private accessTokenSignal = signal<string | null>(null);
  private refreshTokenSignal = signal<string | null>(null);

  user = computed(() => this.userSignal());
  accessToken = computed(() => this.accessTokenSignal());
  refreshToken = computed(() => this.refreshTokenSignal());
  isAuthenticated = computed(() => !!this.accessTokenSignal());

  constructor(private http: HttpClient, private router: Router) {
    this.loadFromStorage();
  }

  private loadFromStorage() {
    const data = localStorage.getItem('hotel-auth-storage');
    if (data) {
      try {
        const parsed = JSON.parse(data);
        if (parsed.user && parsed.accessToken) {
          this.userSignal.set(parsed.user);
          this.accessTokenSignal.set(parsed.accessToken);
          this.refreshTokenSignal.set(parsed.refreshToken);
        }
      } catch (e) {
        this.clearAuth();
      }
    }
  }

  setAuth(user: User, accessToken: string, refreshToken: string) {
    this.userSignal.set(user);
    this.accessTokenSignal.set(accessToken);
    this.refreshTokenSignal.set(refreshToken);
    localStorage.setItem('hotel-auth-storage', JSON.stringify({ user, accessToken, refreshToken }));
  }

  clearAuth() {
    this.userSignal.set(null);
    this.accessTokenSignal.set(null);
    this.refreshTokenSignal.set(null);
    localStorage.removeItem('hotel-auth-storage');
  }

  login(credentials: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(res => {
          if (res.success && res.data) {
            this.setAuth(res.data.user, res.data.accessToken, res.data.refreshToken);
          }
        })
      );
  }

  register(data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/auth/register`, data);
  }

  logout() {
    this.clearAuth();
    this.router.navigate(['/login']);
  }
}
