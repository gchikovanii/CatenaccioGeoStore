﻿using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Infrastructure.DataContext;
using CatenaccioStore.Infrastructure.Errors;
using CatenaccioStore.Infrastructure.Helpers;
using CatenaccioStore.Infrastructure.Repositories.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.API.Infrastructure.Extensions
{
    public static class ApplciationServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<ApplicationDbContext>(options =>
                                                              options.UseSqlite(config.GetConnectionString("DefaultConnectionString")));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
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

            return services;
        }
    }
}