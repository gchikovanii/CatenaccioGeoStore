import { Component, OnInit } from '@angular/core';
import { BasketService } from './services/basket.service';
import { AccountService } from './services/account.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  constructor(private basketService: BasketService, private accountService: AccountService, private translateService: TranslateService){}
  ngOnInit(): void {
    this.loadLang();
    this.loadBasket();
    this.loadCurrentUser();
  }

  loadLang(){
    this.translateService.setDefaultLang('en');
    if (typeof localStorage !== 'undefined') {
      this.translateService.use(localStorage.getItem('lang') || 'en');
    } else {
      this.translateService.use('en');
    }
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
