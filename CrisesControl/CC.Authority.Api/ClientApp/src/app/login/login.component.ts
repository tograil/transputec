import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import jwt_decode from 'jwt-decode';

import { LoginService, TokenData } from '../services/login.service';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  username: string = "";
  password: string = "";

  errorVisible: boolean = false;

  constructor(private loginService:LoginService,
     private router: Router,
      private tokenService: TokenService) { }

  login(): void {
    
    this.loginService.login(this.username, this.password)
      .subscribe({
        next: (res) => {
          this.tokenService.saveToken(res.access_token);
         
          this.router.navigate(['/manage/get-token']);
        },
        error: (e) => { 
          this.errorVisible = true,
          console.log(e);
        },
        complete: () => console.info('complete') 
    });
  }

  ngOnInit(): void {
  }

}
