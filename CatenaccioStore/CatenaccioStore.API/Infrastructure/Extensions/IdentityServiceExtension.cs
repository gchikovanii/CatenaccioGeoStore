using CatenaccioStore.Core.Entities.Identities;
using CatenaccioStore.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatenaccioStore.API.Infrastructure.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("IdentityConnectionString"));
            });
            services.AddIdentityCore<AppUser>(opt =>
            {
                //Password complecity
            }).AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddSignInManager<SignInManager<AppUser>>();
            services.AddAuthentication();
            services.AddAuthorization();

            return services;
        }
    }
}
