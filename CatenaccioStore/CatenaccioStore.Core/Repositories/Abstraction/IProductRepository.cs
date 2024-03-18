﻿using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IProductRepository
    {
        Task<ProductDto> GetProductByIdAsync(CancellationToken token,int id);
        Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken token);
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token);
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token);


    }
}
