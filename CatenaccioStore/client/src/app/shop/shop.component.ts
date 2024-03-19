import { Component, OnInit } from '@angular/core';
import { Product } from '../models/Product';
import { ShopService } from '../services/shop.service';
import { error } from 'console';
import { Brand } from '../models/Brand';
import { Type } from '../models/Type';
import { ShopParams } from '../models/ShopParams';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  products: Product[] = [];
  brands: Brand[] = [];
  types: Type[] = [];
  brandIdSelected = 0;
  typeIdSelected = 0;
  sortSelected = 'name';
  shopParams = new ShopParams();
  sortOptions = [
    {name: 'Alphaberical', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'},
  ];
  totalCount = 0;

  constructor(private shopService: ShopService){}

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }
  
  getProducts(){
    this.shopService.getProducts(this.shopParams).subscribe(
      {
       next: response => {
        this.products = response.data;
        this.shopParams.pageNumber = response.pageIndex;
        this.shopParams.pageSize = response.pageSize;
        this.totalCount = response.count;
       },
       error: error => console.log(error)
      }
     )
  }

  getBrands(){
    this.shopService.getBrands().subscribe(
      {
       next: response => this.brands = [{id:0, name: 'All'}, ...response],
       error: error => console.log(error)
      }
     )
  }
  getTypes(){
    this.shopService.getTypes().subscribe(
      {
       next: response => this.types = [{id:0, name: 'All'}, ...response],
       error: error => console.log(error)
      }
     )
  }

  onBrandSelected(brandId: number){
    this.shopParams.brandId = brandId;
    this.getProducts();
  }

  onTypeSelected(typeId: number){
    this.shopParams.typeId = typeId;
    this.getProducts();
  }
  onSortSelected(event: any){
    this.shopParams.sort = event.target.value;
    this.getProducts();
  }

  onPageChanged(event: any){
    if(this.shopParams.pageNumber !== event.page){
      this.shopParams.pageNumber = event.page;
      this.getProducts();
    }
  }

}
