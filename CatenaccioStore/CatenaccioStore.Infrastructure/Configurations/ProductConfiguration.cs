using CatenaccioStore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatenaccioStore.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(i => i.Id).IsRequired();
            builder.Property(i => i.Name).IsRequired().HasMaxLength(30);
            builder.Property(i => i.Description).IsRequired();
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(i => i.PictureUrl).IsRequired();
            builder.Property(i => i.Quantity).IsRequired();
            builder.HasOne(i => i.ProductBrand).WithMany().HasForeignKey(i => i.ProductBrandId);
            builder.HasOne(i => i.ProductType).WithMany().HasForeignKey(i => i.ProductTypeId);


        }
    }
}
