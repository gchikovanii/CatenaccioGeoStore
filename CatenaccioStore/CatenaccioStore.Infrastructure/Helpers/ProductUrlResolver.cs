using AutoMapper;
using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace CatenaccioStore.Infrastructure.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration _configuration;
        public ProductUrlResolver(IConfiguration config)
        {
            _configuration = config;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return _configuration["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}
