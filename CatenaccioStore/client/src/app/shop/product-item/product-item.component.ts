import { Component, Input } from '@angular/core';
import { Product } from '../../models/Product';
import { faShoppingCart } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.scss'
})
export class ProductItemComponent {
  @Input() product?: Product;
  cart = faShoppingCart;

  constructor(private basketService : BasketService){}

  addItemToBasket(){
    this.product && this.basketService.addItemToBasket(this.product);
  }


}
