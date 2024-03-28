import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RecoveryService  {
  private apiUrl = 'Account';
  constructor(private http: HttpClient) {}

  demandRecovery(email: string): Observable<boolean> {
    return this.http.get<boolean>(`${environment.apiUrl}${this.apiUrl}/SendNotification?email=${email}`);
  }
  resetPassword(token: string, newPassword: string): Observable<boolean> {
    const url = `${environment.apiUrl}${this.apiUrl}/ResetPassword?token=${token}&newPassword=${newPassword}`;
    return this.http.post<boolean>(url, null);
  }

}