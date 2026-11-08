using Microsoft.Extensions.DependencyInjection;
using SmartCatalog.BusinessLogic.Services;

namespace SmartCatalog.BusinessLogic.Extensions;

public static class DepInjExtension
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductService, ProductService>();
        return serviceCollection;
    }
}