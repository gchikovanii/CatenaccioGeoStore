import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DeliveryMethod } from '../models/DeliveryMethod';
import { map } from 'rxjs';
import { Order, OrderToCreate } from '../models/Order';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }


  createOrder(order: OrderToCreate){
    return this.http.post<Order>(this.baseUrl + 'Orders', order);
  }


  getDeliveryMethods(){
    debugger;
    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'Orders/DeliveryMethod').pipe(
      map(dm => {
        return dm.sort((a,b) => b.price - a.price);
      })
    )
  }
}
