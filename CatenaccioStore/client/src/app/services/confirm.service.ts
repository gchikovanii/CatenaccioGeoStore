import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  private apiUrl = 'Account/ConfirmEmail';
  constructor(private http: HttpClient) {}

  confirmEmail(token: string): Observable<boolean> {
    const url = `${environment.apiUrl}${this.apiUrl}?token=${token}`;
    return this.http.post<boolean>(url, null);
  }
}
