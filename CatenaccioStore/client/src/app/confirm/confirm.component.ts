import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmService } from '../services/confirm.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrl: './confirm.component.scss'
})
export class ConfirmComponent  implements OnInit {
  token: string = '';
  errorMessage: string = '';
  isButtonDisabled: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private confirmationService: ConfirmService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
    });
  }
  confirmEmail() {
    this.confirmationService.confirmEmail(this.token).subscribe(
      (success) => {
        if (success) {
          this.openSnackBar('Password confirmed successfully','Success');
        } else {
          this.openSnackBar('Error while confirming password','Failed');
        }
        setTimeout(() => {
          window.location.href = "/login";
        }, 3000);
      },
      (error) => {
        console.error('An error occurred:', error);
        this.openSnackBar('Error while confirming password','Failed');
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

