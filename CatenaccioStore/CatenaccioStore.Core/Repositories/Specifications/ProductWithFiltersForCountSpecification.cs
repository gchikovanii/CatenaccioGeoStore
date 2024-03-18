using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams productSpecParams)
             : base(i => (!productSpecParams.BrandId.HasValue || i.ProductBrandId == productSpecParams.BrandId) &&
            (!productSpecParams.TypeId.HasValue || i.ProductTypeId == productSpecParams.TypeId))
        {
            
        }
    }
}
