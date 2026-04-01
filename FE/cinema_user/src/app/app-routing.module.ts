import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GallaryComponent } from './components/gallary.component';
import { ProductComponent } from './components/product.component';
import { Food_DrinkComponent } from './components/food_drink.component';
import { HomeComponent } from './components/home.component';
import { ShoppingComponent } from './components/shopping.component';
import { Enter_learnComponent } from './components/enter_learn.component';
import { AboutComponent } from './components/about.component';
import { NewsComponent } from './components/news.component';
import { FeedbackComponent } from './components/feedback.component';
import { HomeCinemaComponent } from './components/homecinema.component';
import { BuyTicketComponent } from './components/buy_ticket.component';
import { TicketDetailsComponent } from './components/ticket_details.component';
import { MovieDetailsComponent } from './components/movie_details.component';
import { DatePipe } from '@angular/common';
import { ChatComponent } from './components/chat.component';
import { Login_SignupComponent } from './components/login_signup.component';
import { PricingComponent } from './components/pricing.component';
import { ProfileComponent } from './components/profile.component';
import { CheckLoginService } from './services/checkLogin.service';
import { BlockTicketDetailsService } from './services/blockTicketDetails.service';
import { PaymentReturnComponent } from './components/payment-return.component';
import { ThankYouComponent } from './components/thank-you.component';
import { TicketHistoryComponent } from './components/ticket-history.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'product',
    component: ProductComponent,
  },
  {
    path: 'food-drink',
    component: Food_DrinkComponent,
  },
  {
    path: 'rss-movie',
    component: ShoppingComponent,
  },
  {
    path: 'entertainment-learning',
    component: Enter_learnComponent,
  },
  {
    path: 'about',
    component: AboutComponent,
  },
  {
    path: 'gallery',
    component: GallaryComponent,
  },
  {
    path: 'news',
    component: NewsComponent,
  },

  {
    path: 'feedback',
    component: FeedbackComponent,
  },
  {
    path: 'cinema',
    component: HomeCinemaComponent,
  },
  {
    path: 'buy-ticket/:showId',
    component: BuyTicketComponent,
  },
  ,
  {
    path: 'ticket-details/:paymentId',
    component: TicketDetailsComponent,
    canActivate: [BlockTicketDetailsService],
  },
  {
    path: 'movie-details/:movieId',
    component: MovieDetailsComponent,
  },
  ,
  {
    path: 'chat',
    component: ChatComponent,
    canActivate: [CheckLoginService],
  },
  {
    path: 'login',
    component: Login_SignupComponent,
  },
  {
    path: 'pricing',
    component: PricingComponent,
  },
  { path: 'verify-account', redirectTo: 'login' },
  { path: 'verify', redirectTo: 'login' },
  { path: 'forgot-password', redirectTo: 'login' },
  {
    path: 'profile',
    component: ProfileComponent,
  },
  {
    path: 'payment-return',
    component: PaymentReturnComponent,
  },
  {
    path: 'thank-you/:paymentId',
    component: ThankYouComponent,
  },
  {
    path: 'tickets/history',
    component: TicketHistoryComponent,
    canActivate: [CheckLoginService],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
