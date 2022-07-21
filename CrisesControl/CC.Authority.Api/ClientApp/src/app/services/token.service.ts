import { Injectable } from '@angular/core';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  dayInSeconds:number = 86400;
  constructor() { }

  saveToken(token:string): void {
    sessionStorage.setItem("token", token);
    sessionStorage.setItem("token_saved", moment().format())
  }

  getToken() {
    return sessionStorage.getItem("token");
  }

  validateToken(): boolean {
    let token = sessionStorage.getItem("token");

    if (token == null)
     return false;

    return !this.isExpired();
  }

  isExpired(): boolean {
    let value = sessionStorage.getItem("token_saved");
    if (value === null)
      return true;
    let difference = moment(value).diff(moment(), 'seconds');

    if (difference > this.dayInSeconds) {
      sessionStorage.removeItem("token_saved");
      sessionStorage.removeItem("token");
      return true;
    }
    return false;

  }
}
