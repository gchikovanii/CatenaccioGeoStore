import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import { Basket } from '../models/Basket';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/Product';
import { BaksetItem } from '../models/BasketItem';
import { BasketTotals } from '../models/BasketTotals';
import { DeliveryMethod } from '../models/DeliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiUrl;
  private basketSource = new BehaviorSubject<Basket | null>(null);
  basketSource$ = this.basketSource.asObservable();
  private basketTotalSource  = new BehaviorSubject<BasketTotals | null>(null);
  basketTotalSource$ = this.basketTotalSource.asObservable();
  shipping = 0;
  constructor(private http: HttpClient) { }


  setShippingPrice(deliveryMethod: DeliveryMethod){
    this.shipping = deliveryMethod.price;
    this.calculateTotals();
  }

  getBasket(id:string){
    return this.http.get<Basket>(this.baseUrl + 'Basket?id='+id).subscribe({
      next: basket => {this.basketSource.next(basket); this.calculateTotals()}
    })
  }

  setBasket(basket: Basket){
    return this.http.post<Basket>(this.baseUrl + 'Basket', basket).subscribe({
      next: basket => {this.basketSource.next(basket); this.calculateTotals()}
    })
  }

  getCurrentBasketValue(){
    return this.basketSource.value;
  }

  addItemToBasket(item : Product | BaksetItem, quantity = 1){
    if(this.isProductitem(item)) 
      item = this.mapProductItemToBasketItem(item);
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.baksetItems = this.addOrUpdateItem(basket.baksetItems, item, quantity);
    this.setBasket(basket);
  }
  removeItemFromBasket(id: number, quantity = 1){
    const basket = this.getCurrentBasketValue();
    if(!basket)
      return;
    const item = basket.baksetItems.find(i => i.id === id);
    if(item){
      item.quantity -= quantity;
      if(item.quantity === 0){
        basket.baksetItems = basket.baksetItems.filter(i => i.id !== id);
      }
      if(basket.baksetItems.length > 0)
        this.setBasket(basket);
      else{
        this.deleteBasket(basket);
      }
    }

  }
  deleteBasket(basket: Basket) {
    return this.http.delete(this.baseUrl + 'Basket?id='+basket.id).subscribe({
      next: () => {
       this.deleteLocalBasket();
      }
    });
  }

  deleteLocalBasket(){
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }

  private addOrUpdateItem(baksetItems: BaksetItem[], itemToAdd : BaksetItem, quantity: number): BaksetItem[] {
    const item = baksetItems.find(i => i.id ===itemToAdd.id);
    if(item)
      item.quantity++;
    else{
      itemToAdd.quantity = quantity;
      baksetItems.push(itemToAdd);
    }
    return baksetItems;
  }
  private createBasket(): Basket {
    const basket = new Basket();
    localStorage.setItem('basket_id',basket.id);
    return basket;
  }
  private mapProductItemToBasketItem(item: Product) : BaksetItem{
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      quantity: 0,
      pictureUrl: item.pictureUrl,
      brand: item.productBrand,
      type: item.productType
    }
  }
  private calculateTotals(){
    const basket = this.getCurrentBasketValue();
    if(!basket) 
      return;
    const subtotal = basket.baksetItems.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subtotal + this.shipping;
    this.basketTotalSource.next({shipping: this.shipping,total,subtotal});
  }
  private isProductitem(item: Product | BaksetItem): item is Product {
    return (item as Product).productBrand !== undefined;
  }
}
