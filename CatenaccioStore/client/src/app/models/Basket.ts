import cuid from "cuid";
import { BaksetItem } from "./BasketItem"

export interface Basket {
    id: string
    baksetItems: BaksetItem[],
    shippingPrice: number,
    clientSecret?: string,
    paymentIntentId?: string,
    deliveryMethodId?: number,

}

export class Basket implements Basket {
  id= cuid();
  baksetItems: BaksetItem[] = [];
  shippingPrice = 0;
}

  