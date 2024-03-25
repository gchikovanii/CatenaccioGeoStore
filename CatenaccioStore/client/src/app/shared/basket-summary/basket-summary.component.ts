import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BaksetItem } from '../../models/BasketItem';
import { BasketService } from '../../services/basket.service';
import { faMinusCircle, faPlusCircle, faTrashCan } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-basket-summary',
  templateUrl: './basket-summary.component.html',
  styleUrl: './basket-summary.component.scss'
})
export class BasketSummaryComponent {
  @Output() addItem = new EventEmitter<BaksetItem>();
  @Output() removeItem = new EventEmitter<{id: number, quantity: number}>();
  @Input() isBasket = true;
  minus = faMinusCircle;
  plus = faPlusCircle;
  trash = faTrashCan;

  constructor(public basketService: BasketService){}

  addBasketItem(item: BaksetItem){
    this.addItem.emit(item);
  }

  removeBasketItem(id: number, quantity = 1){
    this.removeItem.emit({id,quantity});
  }


}
