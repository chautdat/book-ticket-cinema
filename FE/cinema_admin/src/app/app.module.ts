
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ChatComponent } from './components/chat.component';
import { BaseUrlService } from './services/baseUrl.service';
import { ChatService } from './services/chatService.service';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Chat } from './models/chat.model';
import { DatePipe } from '@angular/common';
import { LoginComponent } from './components/login.component';
import { DashboardComponent } from './components/dashboard.component';
import { LayoutComponent } from './components/layout.component';
import { CheckLoginService } from './services/checkLogin.service';
import { AccountService } from './services/account.service';
import { AccessDenied } from './components/access_denied.component';
import { CinemaComponent } from './components/cinema.component';
import { CinemaService } from './services/cinema.service';
import { RoomService } from './services/room.service';
import { RoomComponent } from './components/room.component';
import { AddRoomComponent } from './components/add_room.component';
import { ToastModule } from "primeng/toast";
import { ConfirmationService, MessageService } from "primeng/api";
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { EditRoomComponent } from './components/edit_room.component';
import { AccountComponent } from './components/account.component';
import { MovieComponent } from './components/movie.component';
import { MovieService } from './services/movie.service';
import { AddMovieComponent } from './components/add_movie.component';
import { EditMovieComponent } from './components/edit_movie.component';
import { PaymentComponent } from './components/payment.component';
import { PaymentService } from './services/payment.service';
import { SeatComponent } from './components/seat.component';
import { AddSeatComponent } from './components/add_seat.component';
import { SubComponent } from './components/sub.component';
import { AddSubComponent } from './components/add_sub.component';
import { SubService } from './services/sub.service';
import { SeatService } from './services/seat.service';
import { ShowtimeComponent } from './components/showtime.component';
import { AddShowtimeComponent } from './components/add_showtime.component';
import { EditShowtimeComponent } from './components/edit_showtime.component';
import { ShowtimeService } from './services/showtime.service';
import { DropdownModule } from 'primeng/dropdown';
import { RatingComponent } from './components/rating.component';
import { RatingService } from './services/rating.service';
import { RatingModule } from 'primeng/rating';
import { CalendarModule } from 'primeng/calendar';
import { BookingComponent } from './components/booking.component';
import { BookingService } from './services/booking.service';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { AddCinemaComponent } from './components/add_cinema.component';

@NgModule({
  declarations: [
    AppComponent,
    ChatComponent,
    LoginComponent,
    DashboardComponent,
    LayoutComponent,
    AccessDenied,
    CinemaComponent,
    RoomComponent,
    AddRoomComponent,
    EditRoomComponent,
    AccountComponent, 
    MovieComponent,
    AddMovieComponent,
    EditMovieComponent,
    PaymentComponent,
    SeatComponent,
    AddSeatComponent,
    SubComponent,
    AddSubComponent,
    ShowtimeComponent,
    AddShowtimeComponent,
    EditShowtimeComponent,
    RatingComponent,
    BookingComponent,
    AddCinemaComponent

  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    ToastModule,
    BrowserAnimationsModule,
    DropdownModule,
    RatingModule,
    CalendarModule,
    DialogModule,
    FileUploadModule,
    ConfirmDialogModule,
  ],
  providers: [
    BaseUrlService,
    ChatService,
    DatePipe,
    CheckLoginService,
    AccountService,
    CinemaService,
    RoomService,
    MessageService,
    ConfirmationService,
    MovieService,
    PaymentService,
    SubService,
    SeatService,
    ShowtimeService,
    RatingService,
    BookingService
    
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
