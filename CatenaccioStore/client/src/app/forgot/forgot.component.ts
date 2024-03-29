import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RecoveryService } from '../services/recovery.service';

@Component({
  selector: 'app-forgot',
  templateUrl: './forgot.component.html',
  styleUrl: './forgot.component.scss'
})
export class ForgotComponent {
  userEmail: string = '';
  isButtonDisabled: boolean = false;


  constructor(private snackBar: MatSnackBar,private recoveryService: RecoveryService) { }
  demandRecovery(): void {
    if (!this.userEmail || this.userEmail.trim() === '') {
      return;
    }
    this.isButtonDisabled = true;
    this.recoveryService.demandRecovery(this.userEmail).subscribe(
      (success) => {
        if (success) {
          this.openSnackBar('Recovery email was sent successfully, please check email address'); 
          setTimeout(() => {
            window.location.href = "/";
          }, 4000);
        } else {
          this.openSnackBar('Recovery email already was sent or User with current email not exists, please check email address');
          setTimeout(() => {
            window.location.href = "/";
          }, 5000);
        }
      },
      (error) => {
        this.openSnackBar('Recovery email already was sent or User with current email not exists, please check email address');
        setTimeout(() => {
          window.location.href = "/";
        }, 5000);
      }
    );
  }

  openSnackBar(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000 
    });
  
  }
}
