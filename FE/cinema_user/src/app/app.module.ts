import { NgModule } from '@angular/core';
import { BrowserModule, DomSanitizer } from '@angular/platform-browser';
import { ToastModule } from 'primeng/toast';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GallaryComponent } from './components/gallary.component';
import { ProductComponent } from './components/product.component';
import { Food_DrinkComponent } from './components/food_drink.component';
import { HomeComponent } from './components/home.component';
import { ShopAPIService } from './services/shopAPI.service';
import { BaseUrlService } from './services/baseUrl.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ShoppingComponent } from './components/shopping.component';
import { Enter_learnComponent } from './components/enter_learn.component';
import { ProductAPIService } from './services/productAPI.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AboutComponent } from './components/about.component';
import { NewsComponent } from './components/news.component';
import { GallaryAPIService } from './services/gallaryAPI.service';
import { EditorModule } from 'primeng/editor';
import { RatingModule } from 'primeng/rating';
import { ButtonModule } from 'primeng/button';
import { FieldsetModule } from 'primeng/fieldset';
import { InputTextModule } from 'primeng/inputtext';
import { HomeCinemaComponent } from './components/homecinema.component';
import { BuyTicketComponent } from './components/buy_ticket.component';
import { FeedbackAPIService } from './services/feedbackapi.service';
import { FeedbackComponent } from './components/feedback.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ShowAPIService } from './services/showAPI.service';
import { TicketAPIService } from './services/ticketAPI.service';
import { MovieService } from './services/movie.service';
import { DatePipe } from '@angular/common';
import { ShowTimeService } from './services/showTime.service';
import { InputNumberModule } from 'primeng/inputnumber';
import { BookingService } from './services/booking.service';
import { TicketDetailsComponent } from './components/ticket_details.component';
import { QRCodeModule } from 'angularx-qrcode';
import { PaymentService } from './services/payment.service';
import { CinemaService } from './services/cinema.service';
import { MovieDetailsComponent } from './components/movie_details.component';
import { ChatComponent } from './components/chat.component';
import { ChatService } from './services/chatService.service';
import { Login_SignupComponent } from './components/login_signup.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AccountService } from './services/account.service';
import { RssService } from './services/rss.service';
import { ProfileComponent } from './components/profile.component';
import { RatingService } from './services/rating.service';
import { AutoComplete, AutoCompleteModule } from 'primeng/autocomplete';
import { DialogModule } from 'primeng/dialog';
import { CheckLoginService } from './services/checkLogin.service';
import { BlockTicketDetailsService } from './services/blockTicketDetails.service';
import { AuthService } from './services/auth.service';
import { TooltipModule } from 'primeng/tooltip';
import { FollowService } from './services/follow.service';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { PaymentReturnComponent } from './components/payment-return.component';
import { ThankYouComponent } from './components/thank-you.component';
import { TicketHistoryComponent } from './components/ticket-history.component';
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}
@NgModule({
  declarations: [
    AppComponent,
    GallaryComponent,
    ProductComponent,
    Food_DrinkComponent,
    HomeComponent,
    ShoppingComponent,
    Enter_learnComponent,
    ProductComponent,
    AboutComponent,
    NewsComponent,
    GallaryComponent,
    FeedbackComponent,
    HomeCinemaComponent,
    BuyTicketComponent,
    TicketDetailsComponent,
    MovieDetailsComponent,
    ChatComponent,
    Login_SignupComponent,
    ProfileComponent,
    PaymentReturnComponent,
    ThankYouComponent,
    TicketHistoryComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    DropdownModule,
    EditorModule,
    RatingModule,
    ButtonModule,
    FieldsetModule,
    InputTextModule,
    RatingModule,
    ToastModule,
    InputNumberModule,
    QRCodeModule,
    BsDatepickerModule.forRoot(),
    AutoCompleteModule,
    DialogModule,
    TooltipModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),
  ],
  providers: [
    ShopAPIService,
    BaseUrlService,
    ProductAPIService,
    GallaryAPIService,
    FeedbackAPIService,
    MessageService,
    ConfirmationService,
    ShowAPIService,
    TicketAPIService,
    DatePipe,

    //////////////////////
    MovieService,
    ShowTimeService,
    BookingService,
    PaymentService,
    CinemaService,
    ChatService,
    AccountService,
    RssService,
    RatingService,
    CheckLoginService,
    BlockTicketDetailsService,
    AuthService,
    FollowService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
