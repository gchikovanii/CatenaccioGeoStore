import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Order } from '../models/Order';
import { RooItem } from '../models/OrderItem';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  getOrdersForUser() {
  return this.http.get<Order[]>(this.baseUrl + 'Orders');
  }
  getOrderDetailed(id: number) {
    return this.http.get<RooItem>(this.baseUrl + 'Orders/' + id);
  }
}
