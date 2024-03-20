import cuid from "cuid";
import { BaksetItem } from "./BasketItem"

export interface Basket {
    id: string
    baksetItems: BaksetItem[]
}

export class Basket implements Basket {
  id= cuid();
  baksetItems: BaksetItem[] = [];

}


  