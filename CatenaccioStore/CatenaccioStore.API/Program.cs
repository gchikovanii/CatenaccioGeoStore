using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Infrastructure.DaataSeeding;
using CatenaccioStore.Infrastructure.DataContext;
using CatenaccioStore.Infrastructure.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
builder.Services.AddScoped<IProductRepository,ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ApplicationDbContext>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error while migration");
}

app.Run();
