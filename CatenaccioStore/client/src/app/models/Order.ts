import { Address } from "./Address"
import { DeliveryMethod } from "./DeliveryMethod"

export interface OrderToCreate{
    basketId: string;
    deliveryMethodId: number;
    shipToAddress: Address
}

export interface Order {
    buyerEmail: string
    orderDate: string
    shipToAddress: Address
    deliveryMethod: string //Maybe DeliveryMethod?
    orderItems: OrderItem[]
    subtotal: number
    total: number
    orderStatus: number
    paymentIntentId: any
    id: number
  }
  
  export interface OrderItem {
    itemOrdered: ItemOrdered
    price: number
    quantity: number
    id: number
  }
  
  export interface ItemOrdered {
    productItemId: number
    productName: string
    pictureUrl: string
  }
  