using CatenaccioStore.API.DTOs;
using CatenaccioStore.API.Errors;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace CatenaccioStore.API.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<List<ProductDto>>> GetProduct(CancellationToken token, int id)
        {
            var result = await _productRepository.GetProductByIdAsync(token, id);
            if(result == null)
                return NotFound(new ApiResponse(404));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts(CancellationToken token)
        {
            var result = await _productRepository.GetProductsAsync(token);
            if (result == null)
                return NotFound(new ApiResponse(404));
            return Ok(result);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<Product>>> GetProductBrandss(CancellationToken token)
        {
            var result = await _productRepository.GetProductBrandsAsync(token);
            if (result == null)
                return NotFound(new ApiResponse(404));
            return Ok(result);
        }
        [HttpGet("types")]
        public async Task<ActionResult<List<Product>>> GetProductTypes(CancellationToken token)
        {
            var result = await _productRepository.GetProductTypesAsync(token);
            if (result == null)
                return NotFound(new ApiResponse(404));
            return Ok(result);
        }

    }
}
