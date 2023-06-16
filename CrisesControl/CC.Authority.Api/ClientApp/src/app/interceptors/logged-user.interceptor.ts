import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { Router } from '@angular/router';

@Injectable()
export class LoggedUserInterceptor implements HttpInterceptor {

  constructor(private tokenService: TokenService,
    private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    var tokenValid = this.tokenService.validateToken();
    if (tokenValid) {
      var token = this.tokenService.getToken();
      // If we have a token, we set it to the header
      request = request.clone({
         setHeaders: {Authorization: `Bearer ${token}`}
      });
   }

    return next.handle(request).pipe<HttpEvent<any>>(
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
            if (err.status === 401) {
              this.tokenService.removeToken();
         
              this.router.navigate(['/login']);
         }
      }
      return throwError(err);
    }));
  }
}
