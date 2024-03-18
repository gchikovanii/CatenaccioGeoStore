using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }
        public string ProductType { get; set; }
        public string ProductBrand { get; set; }
    }
}
