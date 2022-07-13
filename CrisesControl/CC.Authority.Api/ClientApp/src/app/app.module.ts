import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'; 
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {PasswordModule} from 'primeng/password';
import {MenubarModule} from 'primeng/menubar';
import {InputTextModule} from 'primeng/inputtext';
import {ButtonModule} from 'primeng/button';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {CardModule} from 'primeng/card';
import {DropdownModule} from 'primeng/dropdown';
import {PanelModule} from 'primeng/panel';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { GetTokenComponent } from './manager/get-token/get-token.component';
import { ManagerComponent } from './manager/manager.component';
import { NewUserComponent } from './manager/new-user/new-user.component';

import { LoggedUserInterceptor } from './interceptors/logged-user.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    GetTokenComponent,
    ManagerComponent,
    NewUserComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    PasswordModule,
    MenubarModule,
    InputTextModule,
    ButtonModule,
    CardModule,
    DropdownModule,
    PanelModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoggedUserInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
