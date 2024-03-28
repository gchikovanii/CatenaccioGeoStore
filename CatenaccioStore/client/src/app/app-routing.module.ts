import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ShopComponent } from './shop/shop.component';
import { ProductDetailsComponent } from './shop/product-details/product-details.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { BasketComponent } from './basket/basket.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { authGuard } from './guards/auth.guard';
import { CheckoutSuccessComponent } from './checkout/checkout-success/checkout-success.component';
import { OrdersComponent } from './orders/orders.component';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';
import { ContactComponent } from './contact/contact.component';
import { ConfirmComponent } from './confirm/confirm.component';

const routes: Routes = [
  {path:'', component: HomeComponent},
  {path:'server-error', component: ServerErrorComponent},
  {path:'not-found', component: NotFoundComponent},
  {path:'login', component: LoginComponent},
  {path:'success', component: CheckoutSuccessComponent},
  {path:'register', component: RegisterComponent},
  {path:'basket', component: BasketComponent},
  { path: 'confirmation', component: ConfirmComponent },
  { path: 'confirmation/:token', component: ConfirmComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path:'checkout', component: CheckoutComponent}
    ]
  },
  {path:'contact', component: ContactComponent},
  {path:'shop', component: ShopComponent},
  {path: 'shop/:id', component: ProductDetailsComponent},
  {path:'orders', component: OrdersComponent},
  {path: 'orders/:id', component: OrderDetailedComponent},
  {path:'**', redirectTo: '', pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
