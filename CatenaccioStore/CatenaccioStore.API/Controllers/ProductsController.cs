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

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts(CancellationToken token)
        {
            return Ok(await _productRepository.GetProductsAsync(token));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<List<Product>>> GetProduct(CancellationToken token,int id)
        {
            return Ok(await _productRepository.GetProductByIdAsync(token,id));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<Product>>> GetProductBrandss(CancellationToken token)
        {
            return Ok(await _productRepository.GetProductBrandsAsync(token));
        }
        [HttpGet("types")]
        public async Task<ActionResult<List<Product>>> GetProductTypes(CancellationToken token)
        {
            return Ok(await _productRepository.GetProductTypesAsync(token));
        }

    }
}
