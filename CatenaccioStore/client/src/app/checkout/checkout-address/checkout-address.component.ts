import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { AccountService } from '../../services/account.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrl: './checkout-address.component.scss'
})
export class CheckoutAddressComponent {
  @Input() checkoutForm? : FormGroup;
  leftAngle = faAngleLeft;
  rightAngle = faAngleRight;

  constructor(private accountService: AccountService, private snackBar :MatSnackBar ){}

  saveUserAddress(){
    this.accountService.updateUserAaddress(this.checkoutForm?.get('addressForm')?.value).subscribe({
      next: () => {this.openSnackBar('Address Saved!','Success'); this.checkoutForm?.get('addressForm')?.reset(this.checkoutForm?.get('addressForm')?.value)}
    })
  }

  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 3000, 
    });
  }
}
