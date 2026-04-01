import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Chat } from './models/chat.model';
import { ChatComponent } from './components/chat.component';
import { LoginComponent } from './components/login.component';
import { DashboardComponent } from './components/dashboard.component';
import { LayoutComponent } from './components/layout.component';
import { CheckLoginService } from './services/checkLogin.service';
import { AccessDenied } from './components/access_denied.component';
import { CinemaComponent } from './components/cinema.component';
import { RoomComponent } from './components/room.component';
import { AddRoomComponent } from './components/add_room.component';
import { EditRoomComponent } from './components/edit_room.component';
import { AccountComponent } from './components/account.component';
import { MovieComponent } from './components/movie.component';
import { AddMovieComponent } from './components/add_movie.component';
import { EditMovieComponent } from './components/edit_movie.component';
import { PaymentComponent } from './components/payment.component';
import { SubComponent } from './components/sub.component';
import { AddSubComponent } from './components/add_sub.component';
import { SeatComponent } from './components/seat.component';
import { ShowtimeComponent } from './components/showtime.component';
import { AddShowtimeComponent } from './components/add_showtime.component';
import { EditShowtimeComponent } from './components/edit_showtime.component';
import { RatingComponent } from './components/rating.component';
import { AddSeatComponent } from './components/add_seat.component';
import { BookingComponent } from './components/booking.component';
import { AddCinemaComponent } from './components/add_cinema.component';

const routes: Routes = [
  {
    path: "",
    component: LoginComponent,
  },
  {
    path: "login",
    component: LoginComponent,
  },
  {
    path: "admin",
    component: LayoutComponent,
    canActivate: [CheckLoginService],

    children: [
      {
        path: "chat",
        component: ChatComponent,
      },
     
      {
        path: "dashboard",
        component: DashboardComponent,
      },
      {
        path: "cinema",
        component: CinemaComponent,
      },
      {
        path: "add-cinema",
        component: AddCinemaComponent,
      },
      {
        path: "room",
        component: RoomComponent,
      },
      {
        path: "account",
        component: AccountComponent,
      },
      {
        path: "add-room",
        component: AddRoomComponent,
      },
      {
        path: "edit-room/:roomId",
        component: EditRoomComponent,
      },
      {
        path: "movie",
        component: MovieComponent,
      },
      {
        path: "add-movie",
        component: AddMovieComponent,
      },
      {
        path: "sub",
        component: SubComponent,
      },
      {
        path: "add-sub",
        component: AddSubComponent,
      },
      {
        path: "seat",
        component: SeatComponent,
      },
      {
        path: "add-seat",
        component: AddSeatComponent,
      },
      {
        path: "edit-movie/:movieId",
        component: EditMovieComponent,
      },
      {
        path: "payment",
        component: PaymentComponent,
      },
      {
        path: "showtime",
        component: ShowtimeComponent,
      } ,
      {
        path: "add-showtime",
        component: AddShowtimeComponent,
      } ,
      {
        path: "edit-showtime/:showTimeId",
        component: EditShowtimeComponent,
      },
      {
        path: "rating",
        component: RatingComponent,
      } ,
      {
        path: "booking",
        component: BookingComponent,
      } ,

    ]
  },
  {
    path: "access-denied",
    component: AccessDenied,
  },
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
