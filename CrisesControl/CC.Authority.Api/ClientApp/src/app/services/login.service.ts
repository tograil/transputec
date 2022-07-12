import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private httpClient:HttpClient) { }

  login(username:string, password: string) : Observable<TokenData> {
    const body = new HttpParams()
    .set('grant_type', 'password')
    .set('username', username)
    .set('password', password)
    .set('client_id', '2963ebbf-2cec-4baf-aa97-df3bdb08fe07')
    .set('client_secret', 'b4134e92-b465-4d8d-b339-dd0ff6b31a45');

    return this.httpClient.post<TokenData>("/connect/token", body.toString(),
    {
      headers: new HttpHeaders()
        .set('Content-Type', 'application/x-www-form-urlencoded')
        .set('Accept', '*/*')
    });
  }
}

export interface TokenData {
  access_token: string;
  token_type: string;
  expires_in: number
}
