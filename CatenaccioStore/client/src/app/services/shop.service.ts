import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pagination } from '../models/Pagination';
import { Product } from '../models/Product';
import { environment } from '../../environments/environment';
import { Brand } from '../models/Brand';
import { Type } from '../models/Type';
import { ShopParams } from '../models/ShopParams';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  products: Product[] = [];
  brands: Brand[] = [];
  types: Type[] = [];

  constructor(private http: HttpClient ) { }

  getProducts(shopParams: ShopParams){
    let params = new HttpParams();
    if(shopParams.brandId > 0)
      params = params.append('brandId', shopParams.brandId);
    if(shopParams.typeId > 0)
      params = params.append('typeId', shopParams.typeId);
    if(shopParams.sort)
      params = params.append('sort', shopParams.sort);
    if(shopParams.search)
      params = params.append('search', shopParams.search);
    
    params = params.append('pageIndex', shopParams.pageNumber);
    params = params.append('pageSize', shopParams.pageSize);
    return this.http.get<Pagination<Product>>(this.baseUrl + 'Products',{params}).pipe(
      map(response => {
        this.products = response.data;
        return response;
      })
    )
  }
  getBrands(){
    if(this.brands.length > 0)
      return of(this.brands);
    return this.http.get<Brand[]>(this.baseUrl + 'Products/brands').pipe(
      map(brands => this.brands = brands)
    )
  }
  getTypes(){
    if(this.types.length > 0)
      return of(this.types);
    return this.http.get<Type[]>(this.baseUrl + 'Products/types').pipe(
      map(types => this.types = types)
    )
  }
  getProduct(id: number){
    const product = this.products.find(i => i.id == id);
    if(product)
      return of(product);
    return this.http.get<Product>(this.baseUrl + 'Products/'+id);
  }
}
