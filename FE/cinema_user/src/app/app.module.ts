import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ConfirmationService, MessageService } from 'primeng/api';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home.component';
import { Login_SignupComponent } from './components/login_signup.component';

import { BaseUrlService } from './services/baseUrl.service';
import { AccountService } from './services/account.service';
import { AuthService } from './services/auth.service';
import { CheckLoginService } from './services/checkLogin.service';
import { MovieService } from './services/movie.service';
import { ShowAPIService } from './services/showAPI.service';
import { FollowService } from './services/follow.service';

@NgModule({
  declarations: [AppComponent, HomeComponent, Login_SignupComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AutoCompleteModule,
    ToastModule,
    TooltipModule,
    BsDatepickerModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useClass: TranslateHttpLoader,
      },
    }),
  ],
  providers: [
    BaseUrlService,
    AccountService,
    AuthService,
    CheckLoginService,
    MovieService,
    ShowAPIService,
    FollowService,
    MessageService,
    ConfirmationService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
