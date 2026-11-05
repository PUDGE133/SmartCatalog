using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCatalog.Infrastructure.Data;

namespace SmartCatalog.Infrastructure.Extensions;

public static class DepInjExtension
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        serviceCollection.AddDbContext<AppDbContext>(x =>
        {
            x.UseNpgsql(connectionString);
        });
        return serviceCollection;
    }
}