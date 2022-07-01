import { Component, OnInit } from '@angular/core';
import {MenuItem} from 'primeng/api';
import { UserService, User } from 'src/app/services/user.service';

@Component({
  selector: 'app-get-token',
  templateUrl: './get-token.component.html',
  styleUrls: ['./get-token.component.scss']
})
export class GetTokenComponent implements OnInit {

  users: User[] = [];

  user?:User = undefined;

  token:string = "";

  constructor(private userService: UserService) { 
    
  }

  getToken() {
    this.userService.generateToken(this.user && this.user.id || 0)
        .subscribe(token => {
            this.token = token.access_token;
        })
  }

  ngOnInit(): void {
    this.userService.getSuperadmins()
        .subscribe(users => { 
            this.users = users;
            if(this.users.length > 0)
                this.user = [...this.users].shift();
         });
  }

}
