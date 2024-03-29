import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../models/User';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Address } from '../models/Address';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUserSource$ = this.currentUserSource.asObservable();
  constructor(private http: HttpClient, private router: Router, private matSnackBar: MatSnackBar) { }


  login(values:any){
    return this.http.post<User>(this.baseUrl + 'Account/login',values).pipe(
      map(user => {
        localStorage.setItem('token', user.token);
        this.currentUserSource.next(user);
      })
    )
  }

  register(values: any){
    return this.http.post<User>(this.baseUrl + 'Account/register',values).pipe(
      map(user => {
        // localStorage.setItem('token', user.token);
        // this.currentUserSource.next(user);
        this.openSnackBar('An email confirmation has been sent to your inbox. Please check your email and activate your account by clicking on the provided link!','Successfully');
      })
    )
  }
  loadCurrentUser(token: string){
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`)
    return this.http.get<User>(this.baseUrl + 'Account',{headers}).pipe(
      map(user => {
        localStorage.setItem('token', user.token);
        this.currentUserSource.next(user);
      })
    )
  }
  
  logout(){
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/login');
  }

  checkEmailExists(email: string){
    return this.http.get<boolean>(this.baseUrl + 'Account/emailexists?email='+email);
  }

  getUserAddress(){
    return this.http.get<Address>(this.baseUrl + 'Account/address');
  }
  updateUserAaddress(address:Address){
    return this.http.put<Address>(this.baseUrl + 'Account/address',address);
  }
  openSnackBar(message: string, action: string) {
    this.matSnackBar.open(message, action, {
      duration: 3000, 
    });
  }
}
