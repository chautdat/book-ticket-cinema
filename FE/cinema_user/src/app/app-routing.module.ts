import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './components/home.component';
import { Login_SignupComponent } from './components/login_signup.component';
import { CheckLoginService } from './services/checkLogin.service';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: Login_SignupComponent },

  // Keep legacy links in header from breaking while feature pages are being restored.
  { path: 'news', component: HomeComponent },
  { path: 'cinema', component: HomeComponent },
  { path: 'pricing', component: HomeComponent },
  { path: 'profile', component: HomeComponent },
  { path: 'movie-details/:movieId', component: HomeComponent },
  { path: 'tickets/history', component: HomeComponent, canActivate: [CheckLoginService] },

  { path: '**', redirectTo: 'home' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
