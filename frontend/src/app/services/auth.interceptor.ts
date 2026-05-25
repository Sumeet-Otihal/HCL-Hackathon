import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<any>(null);

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);
  const http = inject(HttpClient);
  const router = inject(Router);

  const token = authService.accessToken();
  
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !authReq.url.includes('/auth/login') && !authReq.url.includes('/auth/refresh')) {
        if (!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(null);

          const refreshToken = authService.refreshToken();
          if (refreshToken) {
            return http.post<any>(`${environment.apiUrl}/auth/refresh`, { refreshToken }).pipe(
              switchMap((res: any) => {
                isRefreshing = false;
                if (res.success && res.data) {
                  authService.setAuth(res.data.user, res.data.accessToken, res.data.refreshToken);
                  refreshTokenSubject.next(res.data.accessToken);
                  return next(req.clone({
                    setHeaders: { Authorization: `Bearer ${res.data.accessToken}` }
                  }));
                }
                authService.logout();
                return throwError(() => new Error('Refresh failed'));
              }),
              catchError((err) => {
                isRefreshing = false;
                authService.logout();
                return throwError(() => err);
              })
            );
          } else {
            isRefreshing = false;
            authService.logout();
          }
        } else {
          return refreshTokenSubject.pipe(
            filter(token => token != null),
            take(1),
            switchMap(jwt => {
              return next(req.clone({
                setHeaders: { Authorization: `Bearer ${jwt}` }
              }));
            })
          );
        }
      }
      return throwError(() => error);
    })
  );
};
