import { Component, OnInit } from '@angular/core';
import { faHistory, faShoppingCart, faSignOut } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../services/basket.service';
import { BaksetItem } from '../models/BasketItem';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent{
  constructor(public basketService: BasketService, public accountService: AccountService){}

  cart = faShoppingCart;
  signout = faSignOut;
  history = faHistory;

  getCount(items: BaksetItem[]){
    return items.reduce((sum,item) => sum + item.quantity,0);
  }

}
