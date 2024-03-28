import { Component, OnInit } from '@angular/core';
import { faBars, faHistory, faShoppingCart, faSignOut, faUser, faUserCircle, faUserPlus } from '@fortawesome/free-solid-svg-icons';
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
  account = faUserCircle;
  loginUser = faUser;
  registerUser = faUserPlus;
  burger = faBars;

  getCount(items: BaksetItem[]){
    return items.reduce((sum,item) => sum + item.quantity,0);
  }

}
