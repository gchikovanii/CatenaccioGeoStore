import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { ShopComponent } from './shop/shop.component';
import { ProductItemComponent } from './shop/product-item/product-item.component';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import { PagingHeaderComponent } from './shared/paging-header/paging-header.component';
import { HomeComponent } from './home/home.component';
import { ProductDetailsComponent } from './shop/product-details/product-details.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { ErrorInterceptor } from './errors/interceptors/error.interceptor';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { LoadingInterceptor } from './errors/interceptors/loading.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BasketComponent } from './basket/basket.component';
import { OrderTotalsComponent } from './shared/order-totals/order-totals.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { AccountComponent } from './account/account.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TextInputComponent } from './shared/text-input/text-input.component';
import {CdkStepperModule} from '@angular/cdk/stepper';
import { StepperComponent } from './shared/stepper/stepper.component';
import { CheckoutAddressComponent } from './checkout/checkout-address/checkout-address.component';
import { CheckoutDeliveryComponent } from './checkout/checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout/checkout-review/checkout-review.component';
import { CheckoutPaymentComponent } from './checkout/checkout-payment/checkout-payment.component';
import { CheckoutSuccessComponent } from './checkout/checkout-success/checkout-success.component';
import { JwtInterceptor } from './errors/interceptors/jwt';
import { BasketSummaryComponent } from './shared/basket-summary/basket-summary.component';
import { OrdersComponent } from './orders/orders.component';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';
import { FooterComponent } from './footer/footer.component';
import { ContactComponent } from './contact/contact.component';
import { ConfirmComponent } from './confirm/confirm.component';


@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    ShopComponent,
    ProductItemComponent,
    PagingHeaderComponent,
    HomeComponent,
    ProductDetailsComponent,
    NotFoundComponent,
    ServerErrorComponent,
    BasketComponent,
    OrderTotalsComponent,
    CheckoutComponent,
    AccountComponent,
    LoginComponent,
    RegisterComponent,
    TextInputComponent,
    StepperComponent,
    CheckoutAddressComponent,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CheckoutPaymentComponent,
    CheckoutSuccessComponent,
    BasketSummaryComponent,
    OrdersComponent,
    OrderDetailedComponent,
    FooterComponent,
    ContactComponent,
    ConfirmComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule, 
    AppRoutingModule,
    FontAwesomeModule,
    HttpClientModule,
    MatSnackBarModule,
    NgxSpinnerModule,
    FormsModule,
    ReactiveFormsModule,
    CdkStepperModule,
    BsDropdownModule.forRoot(),
    PaginationModule.forRoot(),
    TooltipModule.forRoot(),
    ToastrModule.forRoot(
      {
        positionClass: 'toast-bottom-right'
      }
    )
    
  ],
  providers: [
    provideClientHydration(),
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { 
  
}
