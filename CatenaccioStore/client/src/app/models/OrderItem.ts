export interface RooItem {
  id: number
  buyerEmail: string
  orderDate: string
  shipToAddress: ShipToAddress
  deliveryMethod: string
  shippingPrice: number
  orderItems: OrderItem[]
  subtotal: number
  total: number
  orderStatus: string
}

export interface ShipToAddress {
  firstName: string
  lastName: string
  city: string
  street: string
  zipCode: string
}

export interface OrderItem {
  productId: number
  productName: string
  pictureUrl: string
  price: number
  quantity: number
}