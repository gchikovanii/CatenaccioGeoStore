using AutoMapper;
using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.DTOs;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Identities;
using CatenaccioStore.Core.Entities.Orders;

namespace CatenaccioStore.Infrastructure.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(i => i.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(i => i.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(i => i.PictureUrl, o => o.MapFrom<ProductUrlResolver>());
            CreateMap<Core.Entities.Identities.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BaksetItemDto, BaksetItem>();
            CreateMap<AddressDto, Core.Entities.Orders.Address>();
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(i => i.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(i => i.ShippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(i => i.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(i => i.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(i => i.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(i => i.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());

        }
    }
}
