import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../models/Product';
import { faShoppingCart } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.scss'
})
export class ProductItemComponent implements OnInit {
  @Input() product?: Product;
  cart = faShoppingCart;  
  lang!: string | null;

  constructor(private basketService : BasketService){}
  ngOnInit(): void {
    this.lang = localStorage.getItem('lang');
  }

  addItemToBasket(){
    this.product && this.basketService.addItemToBasket(this.product);
  }


}
