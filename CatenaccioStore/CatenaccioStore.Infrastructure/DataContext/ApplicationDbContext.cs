﻿using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.Infrastructure.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<PasswordRecoveryToken> PasswordRecoveryToken { get; set; }
        public DbSet<UserConfirmationToken> UserConfirmationToken { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            if(Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var item in modelBuilder.Model.GetEntityTypes())
                {
                    var proprerties = item.ClrType.GetProperties().Where(i => i.PropertyType == typeof(decimal));
                    foreach (var property in proprerties)
                    {
                        modelBuilder.Entity(item.Name).Property(property.Name).HasConversion<double>();
                    }
                }
            }
        }

    }
}

