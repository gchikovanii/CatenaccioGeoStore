using CatenaccioStore.API.Infrastructure.Extensions;
using CatenaccioStore.API.Infrastructure.Middleware;
using CatenaccioStore.API.Infrastructure.Models;
using CatenaccioStore.Core.Entities.Identities;
using CatenaccioStore.Infrastructure.DaataSeeding;
using CatenaccioStore.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.Configure<GoogleAppSettings>(builder.Configuration.GetSection("GoogleAppSettings"));
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ApplicationDbContext>();
var identityContext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await context.Database.MigrateAsync();
    await identityContext.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error while migration");
}

app.Run();
