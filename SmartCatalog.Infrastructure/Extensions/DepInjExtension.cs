using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCatalog.Infrastructure.Cache;
using SmartCatalog.Infrastructure.Repositories;

namespace SmartCatalog.Infrastructure.Extensions;

public static class DepInjExtension
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        serviceCollection.AddDbContext<AppDbContext>(x => x.UseNpgsql(connectionString));
        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<ICacheService, RedisCacheService>();
        return serviceCollection;
    }
}