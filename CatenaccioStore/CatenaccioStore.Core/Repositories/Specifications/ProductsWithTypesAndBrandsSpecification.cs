using CatenaccioStore.Core.Entities;
using System.Linq.Expressions;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productSpecParams) 
            : base(i => (!productSpecParams.BrandId.HasValue || i.ProductBrandId == productSpecParams.BrandId) && 
            (!productSpecParams.TypeId.HasValue || i.ProductTypeId == productSpecParams.TypeId))
        {
            AddInclude(i => i.ProductType);
            AddInclude(i => i.ProductBrand);
            AddOrderBy(i => i.Name);
            ApplyPaging(productSpecParams.PageSize * (productSpecParams.PageIndex - 1), productSpecParams.PageSize);

            if (!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc": 
                           AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(i => i.Name);
                        break;
                }
            }


        }

        public ProductsWithTypesAndBrandsSpecification(int id) : base(i => i.Id == id)
        {
            AddInclude(i => i.ProductType);
            AddInclude(i => i.ProductBrand);
        }
    }
}
