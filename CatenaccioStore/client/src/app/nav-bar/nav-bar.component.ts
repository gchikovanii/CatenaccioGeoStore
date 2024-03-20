import { Component, OnInit } from '@angular/core';
import { faShoppingCart } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../services/basket.service';
import { BaksetItem } from '../models/BasketItem';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent implements OnInit{
  constructor(public basketService: BasketService){}
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  cart = faShoppingCart;


  getCount(items: BaksetItem[]){
    return items.reduce((sum,item) => sum + item.quantity,0);
  }

}
