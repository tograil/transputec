import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { GetTokenComponent } from './manager/get-token/get-token.component';
import { ManagerComponent } from './manager/manager.component';
import { UserLoggedInGuard } from './guards/user-logged-in.guard';
import { NewUserComponent } from './manager/new-user/new-user.component';

const routes: Routes = [
  { path: '',   redirectTo: '/manage/get-token', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { 
    path: 'manage', component: ManagerComponent, canActivate: [ UserLoggedInGuard ],
    children: [
      { path: 'new-user', component: NewUserComponent },
      { path: 'get-token', component: GetTokenComponent }
    ] 
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
