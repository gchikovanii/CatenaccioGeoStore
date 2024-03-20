import { Component, OnInit } from '@angular/core';
import { BasketService } from './services/basket.service';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  constructor(private basketService: BasketService, private accountService: AccountService){}
  ngOnInit(): void {
    this.loadBasket();
    this.loadCurrentUser();
  }

  loadBasket(){
    const basketId = localStorage.getItem('basket_id');
    if(basketId) this.basketService.getBasket(basketId);
  }
  loadCurrentUser(){
    const token = localStorage.getItem('token')
    if(token)
      this.accountService.loadCurrentUser(token).subscribe();
  }


}
