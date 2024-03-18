﻿using AutoMapper;
using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.Entities;

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

        }
    }
}