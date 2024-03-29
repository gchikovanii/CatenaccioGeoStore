import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faAngleLeft, faAngleRight, faCheckCircle, faSpinner } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { CheckoutService } from '../../services/checkout.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Basket } from '../../models/Basket';
import { Address } from '../../models/Address';
import { NavigationExtras, Router } from '@angular/router';
import { Stripe, StripeCardCvcElement, StripeCardExpiryElement, StripeCardNumberElement, loadStripe } from '@stripe/stripe-js';
import { __values } from 'tslib';
import { firstValueFrom } from 'rxjs';
import { OrderToCreate } from '../../models/Order';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrl: './checkout-payment.component.scss'
})
export class CheckoutPaymentComponent implements OnInit, AfterViewInit{
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;
  spinner = faSpinner;
  @Input() checkoutForm?: FormGroup;
  @ViewChild('cardNumber') cardNumberElement?: ElementRef;
  @ViewChild('cardExpiry') cardExpiryElement?: ElementRef;
  @ViewChild('cardCvc') cardCvcElement?: ElementRef;
  stripe: Stripe | null = null;
  cardNumber?: StripeCardNumberElement;
  cardExpiry?: StripeCardExpiryElement;
  cardCvc?: StripeCardCvcElement;

  cardNumberComplete = false;
  cardExpiryComplete = false;
  cardCvcComplete = false;

  cardErrors: any;
  loading = false;


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
          this.cardNumberComplete = event.complete;
          if(event.error)
            this.cardErrors = event.error.message;
          else
            this.cardErrors = null;
        })
        this.cardExpiry.on('change',event => {
          this.cardExpiryComplete = event.complete;
          if(event.error)
            this.cardErrors = event.error.message;
          else
            this.cardErrors = null;
        })
        this.cardCvc.on('change',event => {
          this.cardCvcComplete = event.complete;
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

  get paymentFormComplete(){
    return this.checkoutForm?.get('paymentForm')?.valid && this.cardNumberComplete && this.cardCvcComplete && this.cardExpiryComplete;
  }
  async submitOrder(){
    this.loading = true;
    const basket = this.basketService.getCurrentBasketValue();
    if(!basket)
      throw new Error('cannot get basket')
    try{
      const createdOrder = await this.createOrder(basket);
      const paymentResult = await this.confirmPaymentWithStripe(basket);
      if(paymentResult.paymentIntent){
        this.basketService.deleteBasket(basket);
        const navigationExtras: NavigationExtras = {state: createdOrder};
        this.router.navigate(['success'],navigationExtras);
      }
      else{
        var message = paymentResult.error.message;
        this.openSnackBar(''+ message,'failed')
      }
    }
    catch(error: any){
      this.openSnackBar(''+ error.message,'failed')
    }
    finally{
      this.loading = false;
    }
  }
  private async confirmPaymentWithStripe(basket: Basket | null) {
    if(!basket)
      throw new Error('Basket is null');
    const result =  this.stripe?.confirmCardPayment(basket.clientSecret!,{
      payment_method: {
        card: this.cardNumber!,
        billing_details:{
          name: this.checkoutForm?.get('paymentForm')?.get('nameOnCard')?.value
        }
      }
    });
    if(!result)
      throw new Error('Problem attempting payment with stripe');
    return result; 
  }
  private async createOrder(basket: Basket | null) {
    if(!basket)
      throw new Error('Basket is null');
    const orderToCreate = this.getOrderToCreate(basket);
    return firstValueFrom(this.checkoutService.createOrder(orderToCreate));
  }


  private getOrderToCreate(basket: Basket) : OrderToCreate {
    const deliveryMethodId = this.checkoutForm?.get('deliveryForm')?.get('deliveryMethod')?.value;
    const shipToAddress = this.checkoutForm?.get('addressForm')?.value as Address;
    if(!deliveryMethodId || !shipToAddress)
      throw new Error('Problem with basket');
    return {
      basketId: basket.id,
      deliveryMethodId: deliveryMethodId,
      shipToAddress: shipToAddress
    }
  }
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 3000, 
    });
  }
}
