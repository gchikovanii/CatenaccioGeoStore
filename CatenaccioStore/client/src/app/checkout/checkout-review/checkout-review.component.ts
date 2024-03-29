import { Component, Input } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CdkStepper } from '@angular/cdk/stepper';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;
  @Input() appStepper?: CdkStepper;
  
  constructor(private basketService: BasketService, private snackBar: MatSnackBar){}

  createPaymentIntent(){
    this.basketService.createPaymentIntent().subscribe({
      next: () => {this.appStepper?.next()},
      error: error => this.openSnackBar('' + error.message,'error')
    })
  }
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 3000, 
    });
  }
}
