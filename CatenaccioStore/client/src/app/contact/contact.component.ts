import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss'
})
export class ContactComponent {
  formData: any = {};
  responseMessage: string = '';
  enviroment = environment.apiUrl;
  constructor(private http: HttpClient,private snackBar: MatSnackBar) {}
  submitForm() {
    this.http.post<any>(`${this.enviroment}Contact`, this.formData).subscribe(
      response => {
        this.responseMessage = 'Message sent successfully';
        this.openSnackBar('Message sent successfully', 'Success');
        this.formData = {};
      },
      error => {
        this.responseMessage = 'Error: Message could not be sent';
        this.openSnackBar('Message sent successfully', 'Success');
        this.formData = {};


      }
    );
  }
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 4000, 
    });
  }
}
