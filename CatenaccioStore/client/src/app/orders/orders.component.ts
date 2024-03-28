import { Component, OnInit } from '@angular/core';
import { Order } from '../models/Order';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  constructor(private orderService: OrderService) { }
  ngOnInit(): void {
  this.getOrders();
  }
  getOrders() {
  this.orderService.getOrdersForUser().subscribe({
  next: orders => this.orders = orders
  
  })
  }
}
