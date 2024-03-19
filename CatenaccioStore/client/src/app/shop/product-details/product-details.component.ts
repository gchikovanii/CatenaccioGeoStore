import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../services/shop.service';
import { Product } from '../../models/Product';
import { ActivatedRoute } from '@angular/router';
import { faMinusCircle, faPlusCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit{
  product?: Product;
  constructor(private shopService: ShopService, private activatedRoute : ActivatedRoute){}
  minus = faMinusCircle;
  plus = faPlusCircle;
  ngOnInit(): void {
    this.loadProduct();
  }
  loadProduct(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if(id)
      this.shopService.getProduct(+id).subscribe({
      next: product => this.product = product,
      error: error => console.log(error)
    })
  }

}
