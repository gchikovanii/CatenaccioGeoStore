import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faAngleLeft, faAngleRight, faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { CheckoutService } from '../../services/checkout.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Basket } from '../../models/Basket';
import { Address } from '../../models/Address';
import { NavigationExtras, Router } from '@angular/router';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss'
})
export class CheckoutPaymentComponent {
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;
  @Input() checkoutForm?: FormGroup;

  constructor(private basketService: BasketService, private router: Router, private checkoutService: CheckoutService, private snackBar: MatSnackBar){}

  submitOrder(){
    const basket = this.basketService.getCurrentBasketValue();
    if(!basket)
      return;
    const orderToCreate = this.getOrderToCreate(basket);
    if(!orderToCreate)
      return;
    this.checkoutService.createOrder(orderToCreate).subscribe({
      next: order => {
        this.snackBar.open('Order created successfully')
        this.basketService.deleteLocalBasket();
        const navigationExtras: NavigationExtras = {state: order};
        this.router.navigate(['success'],navigationExtras);
      }
    })
  }
  private getOrderToCreate(basket: Basket) {
    const deliveryMethodId = this.checkoutForm?.get('deliveryForm')?.get('deliveryMethod')?.value;
    const shipToAddress = this.checkoutForm?.get('addressForm')?.value as Address;
    if(!deliveryMethodId || !shipToAddress)
      return;
    return {
      basketId: basket.id,
      deliveryMethodId: deliveryMethodId,
      shipToAddress: shipToAddress
    }
  }

}
