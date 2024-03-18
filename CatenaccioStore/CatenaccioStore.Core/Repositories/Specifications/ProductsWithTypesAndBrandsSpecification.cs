using CatenaccioStore.Core.Entities;
using System.Linq.Expressions;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification()
        {
            AddInclude(i => i.ProductType);
            AddInclude(i => i.ProductBrand);
        }

        public ProductsWithTypesAndBrandsSpecification(int id) : base(i => i.Id == id)
        {
            AddInclude(i => i.ProductType);
            AddInclude(i => i.ProductBrand);
        }
    }
}
