import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pagination } from '../models/Pagination';
import { Product } from '../models/Product';
import { environment } from '../../environments/environment';
import { Brand } from '../models/Brand';
import { Type } from '../models/Type';
import { ShopParams } from '../models/ShopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient ) { }

  getProducts(shopParams: ShopParams){
    let params = new HttpParams();
    if(shopParams.brandId > 0)
      params = params.append('brandId', shopParams.brandId);
    if(shopParams.typeId > 0)
      params = params.append('typeId', shopParams.typeId);
    if(shopParams.sort)
      params = params.append('sort', shopParams.sort);

    params = params.append('pageIndex', shopParams.pageNumber);
    params = params.append('pageSize', shopParams.pageSize);
    return this.http.get<Pagination<Product>>(this.baseUrl + 'Products',{params});
  }
  getBrands(){
    return this.http.get<Brand[]>(this.baseUrl + 'Products/brands');
  }
  getTypes(){
    return this.http.get<Type[]>(this.baseUrl + 'Products/types');
  }
}
