import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DeliveryMethod } from '../../models/DeliveryMethod';
import { CheckoutService } from '../../services/checkout.service';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';

@Component({
  selector: 'app-checkout-delivery',
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent  implements OnInit{
  @Input() checkoutForm?: FormGroup;
  deliveryMethods: DeliveryMethod[] = [];
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;

  constructor(private checkoutService:CheckoutService, private basketService: BasketService){}
  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe({
      next: dm => this.deliveryMethods = dm
    })
  }

  setShippingPrice(deliveryMethod: DeliveryMethod){
    this.basketService.setShippingPrice(deliveryMethod);
  }

}
