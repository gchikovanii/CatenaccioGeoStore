using CatenaccioStore.API.Infrastructure.Helpers;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Infrastructure.DataContext;
using CatenaccioStore.Infrastructure.Errors;
using CatenaccioStore.Infrastructure.Helpers;
using CatenaccioStore.Infrastructure.Repositories.Implementation;
using CatenaccioStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CatenaccioStore.API.Infrastructure.Extensions
{
    public static class ApplciationServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                                                              options.UseNpgsql(config.GetConnectionString("DefaultConnectionString")));
            services.AddSingleton<IConnectionMultiplexer>(i =>
            {
                var options = ConfigurationOptions.Parse(config.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPasswordRecoveryTokenRepository, PasswordRecoveryTokenRepository>();
            services.AddScoped<IUserConfirmationRepository, UserConfirmationRepository>();
            services.AddScoped<IUserConfirmationService, UserConfirmationService>();
            services.AddScoped<IPasswordRecoveryTokenService, PasswordRecoveryTokenService>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<TokenGeneratorService>(provider =>
                new TokenGeneratorService("vfxoUyBgWZQ3glnh64aV9Q8MRtv1X8zwnvC+67Uk9qs=", 60));
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddAutoMapper(typeof(MappingProfiles)); 
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(i => i.Value.Errors.Count > 0)
                        .SelectMany(i => i.Value.Errors)
                        .Select(i => i.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
            return services;
        }
    }
}
