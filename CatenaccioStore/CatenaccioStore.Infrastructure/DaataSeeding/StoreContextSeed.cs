using CatenaccioStore.Core.Entities;
using CatenaccioStore.Infrastructure.DataContext;
using System.Text.Json;

namespace CatenaccioStore.Infrastructure.DaataSeeding
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../CatenaccioStore.Infrastructure/DaataSeeding/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                context.ProductBrands.AddRange(brands);
            }
            if (!context.ProductTypes.Any())
            {
                var typeData = File.ReadAllText("../CatenaccioStore.Infrastructure/DaataSeeding/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                context.ProductTypes.AddRange(types);
            }
            if (!context.ProductBrands.Any())
            {
                var productsData = File.ReadAllText("../CatenaccioStore.Infrastructure/DaataSeeding/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                context.Products.AddRange(products);
            }
            if (context.ChangeTracker.HasChanges())
                await context.SaveChangesAsync();
        }
    }
}
