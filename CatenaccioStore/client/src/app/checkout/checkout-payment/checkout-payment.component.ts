import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faAngleLeft, faAngleRight, faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { CheckoutService } from '../../services/checkout.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Basket } from '../../models/Basket';
import { Address } from '../../models/Address';
import { NavigationExtras, Router } from '@angular/router';
import { Stripe, StripeCardCvcElement, StripeCardExpiryElement, StripeCardNumberElement, loadStripe } from '@stripe/stripe-js';
import { __values } from 'tslib';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss'
})
export class CheckoutPaymentComponent implements OnInit, AfterViewInit{
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;
  @Input() checkoutForm?: FormGroup;
  @ViewChild('cardNumber') cardNumberElement?: ElementRef;
  @ViewChild('cardExpiry') cardExpiryElement?: ElementRef;
  @ViewChild('cardCvc') cardCvcElement?: ElementRef;
  stripe: Stripe | null = null;
  cardNumber?: StripeCardNumberElement;
  cardExpiry?: StripeCardExpiryElement;
  cardCvc?: StripeCardCvcElement;
  cardErrors: any;
  constructor(private basketService: BasketService, private router: Router, private checkoutService: CheckoutService, private snackBar: MatSnackBar){}
  ngAfterViewInit(): void {
    loadStripe('pk_test_51OyTGYRuZrvhruxsYFDynSr6O35WoZVJW3qJzt9J7Otjsv8nY6Px8RDXAxczptWA5Yg7mE7MkvwEgeyZyDSUhb5w00VU10Ju5x').then(stripe => {
      this.stripe = stripe;
      console.log('Stripe loaded:', stripe);
      const elements = stripe?.elements();
      if (elements) {
        this.cardNumber = elements.create('cardNumber');
        this.cardExpiry = elements.create('cardExpiry');
        this.cardCvc = elements.create('cardCvc');
  
        this.cardNumber.mount(this.cardNumberElement?.nativeElement);
        this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
        this.cardCvc.mount(this.cardCvcElement?.nativeElement);

        this.cardNumber.on('change',event => {
          if(event.error)
            this.cardErrors = event.error.message;
          else
            this.cardErrors = null;
        })
        this.cardExpiry.on('change',event => {
          if(event.error)
            this.cardErrors = event.error.message;
          else
            this.cardErrors = null;
        })
        this.cardCvc.on('change',event => {
          if(event.error)
            this.cardErrors = event.error.message;
          else
            this.cardErrors = null;
        })
      }
    });
  }
  
  ngOnInit(): void {
  }


  submitOrder(){
    const basket = this.basketService.getCurrentBasketValue();
    if(!basket)
      return;
    const orderToCreate = this.getOrderToCreate(basket);
    if(!orderToCreate)
      return;
    this.checkoutService.createOrder(orderToCreate).subscribe({
      next: order => {
        this.snackBar.open('Order created successfully');
        this.stripe?.confirmCardPayment(basket.clientSecret!,{
          payment_method: {
            card: this.cardNumber!,
            billing_details:{
              name: this.checkoutForm?.get('paymentForm')?.get('nameOnCard')?.value
            }
          }
        }).then(result => {
          if(result.paymentIntent){
            this.basketService.deleteLocalBasket();
            const navigationExtras: NavigationExtras = {state: order};
            this.router.navigate(['success'],navigationExtras);
          }
        })
       
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
