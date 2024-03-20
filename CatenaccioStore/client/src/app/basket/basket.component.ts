import { Component } from '@angular/core';
import { BasketService } from '../services/basket.service';
import { faMinusCircle, faPlusCircle, faTrashCan } from '@fortawesome/free-solid-svg-icons';
import { BaksetItem } from '../models/BasketItem';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.scss'
})
export class BasketComponent {
  minus = faMinusCircle;
  plus = faPlusCircle;
  trash = faTrashCan;
  constructor(public basketService: BasketService){}


  incrementQuantity(item: BaksetItem){
    this.basketService.addItemToBasket(item);
  }
  removeItem(id: number, quantity: number){
    this.basketService.removeItemFromBasket(id,quantity);
  }


}
