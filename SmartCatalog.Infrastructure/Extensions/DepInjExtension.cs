using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCatalog.Infrastructure.Cache;
using SmartCatalog.Infrastructure.Repositories;
using StackExchange.Redis;

namespace SmartCatalog.Infrastructure.Extensions;

public static class DepInjExtension
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var postgresqlConStr = configuration.GetConnectionString("PostgreSQL");
        serviceCollection.AddDbContext<AppDbContext>(x => x.UseNpgsql(postgresqlConStr));

        serviceCollection.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConStr = configuration.GetConnectionString("Redis");
            var options = ConfigurationOptions.Parse(redisConStr!);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });

        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
        return serviceCollection;
    }
}