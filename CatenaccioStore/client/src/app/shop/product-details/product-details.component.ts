import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../services/shop.service';
import { Product } from '../../models/Product';
import { ActivatedRoute } from '@angular/router';
import { faMinusCircle, faPlusCircle } from '@fortawesome/free-solid-svg-icons';
import { BasketService } from '../../services/basket.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit{
  product?: Product;
  constructor(private shopService: ShopService, private activatedRoute : ActivatedRoute, private basketService: BasketService){}
  minus = faMinusCircle;
  plus = faPlusCircle;
  quantity = 1;
  quantityInBasket = 0;

  ngOnInit(): void {
    this.loadProduct();
  }
  loadProduct(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(id)
      this.shopService.getProduct(+id).subscribe({
      next: product => 
      {
        this.product = product;
        this.basketService.basketSource$.pipe(take(1)).subscribe({
          next: basket => {
            const item = basket?.baksetItems.find(i => i.id === +id)
            if(item){
              this.quantity = item.quantity;
              this.quantityInBasket = item.quantity;
            }
          }
        })
      },
    })
  }

  inerementQuantity(){
    if(this.product){
      if(this.product?.quantity > this.quantity)
      this.quantity++;
    }
  }
  decerementQuantity(){
    if(this.quantity > 0)
      this.quantity--;
  }

  updateBasket(){
    if(this.product){
      if(this.quantity > this.quantityInBasket){
        const itemsToAdd = this.quantity - this.quantityInBasket;
        this.quantityInBasket += itemsToAdd;
        this.basketService.addItemToBasket(this.product,itemsToAdd);
      }
      else{
        const itemsToRemove = this.quantityInBasket - this.quantity;
        this.quantityInBasket -= itemsToRemove;
        this.basketService.removeItemFromBasket(this.product.id,itemsToRemove);
      }
    }
  }

  get buttonText(){
    return this.quantityInBasket === 0 ? 'Add to basket' : 'Update basket'
  }



}
