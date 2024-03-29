import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Product } from '../models/Product';
import { ShopService } from '../services/shop.service';
import { error } from 'console';
import { Brand } from '../models/Brand';
import { Type } from '../models/Type';
import { ShopParams } from '../models/ShopParams';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  @ViewChild('search') searchTerm? : ElementRef;
  products: Product[] = [];
  brands: Brand[] = [];
  types: Type[] = [];
  brandIdSelected = 0;
  typeIdSelected = 0;
  sortSelected = 'name';
  shopParams: ShopParams;

  sortOptions = [
    {name: 'Alphaberical', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'},
  ];
  totalCount = 0;

  constructor(private shopService: ShopService, private matSnack: MatSnackBar){
    this.shopParams = shopService.getShopParams();
  }

  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }
  
  getProducts(){
    this.shopService.getProducts().subscribe(
      {
       next: response => {
        this.products = response.data;
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
    const params = this.shopService.getShopParams();
    params.brandId = brandId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }

  onTypeSelected(typeId: number){
    const params = this.shopService.getShopParams();
    params.typeId = typeId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }
  onSortSelected(event: any){
    const params = this.shopService.getShopParams();
    params.sort = event.target.value;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }

  onPageChanged(event: any){
    const params = this.shopService.getShopParams();
    if(params.pageNumber !== event.page){
      params.pageNumber = event.page;
      this.shopService.setShopParams(params);
      this.shopParams = params;
      this.getProducts();
    }
  }

  onSearch(){
    const params = this.shopService.getShopParams();
    params.search = this.searchTerm?.nativeElement.value;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.shopParams = params;
    this.getProducts();
  }

  onReset(){
    if(this.searchTerm)
      this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }


  

}
