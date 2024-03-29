using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;
using CatenaccioStore.Infrastructure.DataContext;
using System.Reflection;
using System.Text.Json;

namespace CatenaccioStore.Infrastructure.DaataSeeding
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!context.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText(path + @"/DaataSeeding/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                context.ProductBrands.AddRange(brands);
            }
            if (!context.ProductTypes.Any())
            {
                var typeData = File.ReadAllText(path + @"/DaataSeeding/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                context.ProductTypes.AddRange(types);
            }
            if (!context.ProductBrands.Any())
            {
                var productsData = File.ReadAllText(path + @"/DaataSeeding/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                context.Products.AddRange(products);
            }
            if (!context.DeliveryMethods.Any())
            {
                var deliveryMethodsData = File.ReadAllText(path + @"/DaataSeeding/SeedData/delivery.json");
                var delivery = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                context.DeliveryMethods.AddRange(delivery);
            }
            if (context.ChangeTracker.HasChanges())
                await context.SaveChangesAsync();
        }
    }
}
