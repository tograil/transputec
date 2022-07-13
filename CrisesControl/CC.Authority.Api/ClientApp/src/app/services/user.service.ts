import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) { }

  getSuperadmins() : Observable<User[]> {
    return this.httpClient.get<User[]>("/User/superadmins");
  }

  generateToken(userId: number)
  {
    const body = new HttpParams()
    .set('grant_type', 'client_credentials')
    .set('client_id', '2963ebbf-2cec-4baf-aa97-df3bdb08fe07')
    .set('client_secret', 'b4134e92-b465-4d8d-b339-dd0ff6b31a45');
    
    return this.httpClient.post<UserToken>(`/User/token?id=${userId}`, body);
  }

}

export interface User {
  id: number;
  login: string;
  name: string;
}

export interface UserToken {
  access_token: string;
  token_type: string;
  expires_in: number
}
