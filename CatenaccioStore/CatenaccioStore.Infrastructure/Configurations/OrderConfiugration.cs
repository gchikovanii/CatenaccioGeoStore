using CatenaccioStore.Core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatenaccioStore.Infrastructure.Configurations
{
    public class OrderConfiugration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(i => i.ShipToAddress, o => o.WithOwner());
            builder.Navigation(i => i.ShipToAddress).IsRequired();  
            builder.Property(i => i.OrderStatus).HasConversion(i => i.ToString(), i => (OrderStatus)Enum.Parse(typeof(OrderStatus),i));
            builder.HasMany(i => i.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
