import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RecoveryService } from '../services/recovery.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-recovery',
  templateUrl: './recovery.component.html',
  styleUrl: './recovery.component.scss'
})
export class RecoveryComponent implements OnInit {
  token: string = '';
  password: string = '';
  rePassword: string = '';
  errorMessage: string = '';
  newPassword: string = '';
  isButtonDisabled: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private passwordResetService: RecoveryService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
    });
  }
  resetPassword() {
    if (this.newPassword !== this.rePassword) {
      this.openSnackBar('Passwords do not match','Failed');
      return;
    }
    this.isButtonDisabled = true;
    this.passwordResetService.resetPassword(this.token, this.newPassword).subscribe(
      (success) => {
        if (success) {
          this.openSnackBar('Password reset successfully','Success');
        } else {
          this.openSnackBar('Error while resetting password','Failed');
        }
        setTimeout(() => {
          window.location.href = "/login";
        }, 3000);
      },
      (error) => {
        console.error('An error occurred:', error);
        this.openSnackBar('Error while resetting password','Failed');
        setTimeout(() => {
          window.location.href = "/login";
        }, 3000);
      }
    );
  }
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 3000, 
    });
  }
}
