using CatenaccioStore.Core.Entities;
using System.Linq.Expressions;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(string sort)
        {
            AddInclude(i => i.ProductType);
            AddInclude(i => i.ProductBrand);
            AddOrderBy(i => i.Name);

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
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
