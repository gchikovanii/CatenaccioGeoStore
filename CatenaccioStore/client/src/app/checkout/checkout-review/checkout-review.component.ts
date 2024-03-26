import { Component } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;

  constructor(private basketService: BasketService, private snackBar: MatSnackBar){}

  createPaymentIntent(){
    this.basketService.createPaymentIntent().subscribe({
      next: () => this.snackBar.open('Payment intent success'),
      error: error => this.snackBar.open(error.message),
    })
  }

}
