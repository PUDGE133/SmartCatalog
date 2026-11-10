using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using SmartCatalog.BusinessLogic.Consumers;
using SmartCatalog.BusinessLogic.Consumers.Invalidators;
using SmartCatalog.BusinessLogic.Services;

namespace SmartCatalog.BusinessLogic.Extensions;

public static class DepInjExtension
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ProductCacheInvalidator>();

        serviceCollection.AddMassTransit(x =>
        {
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<ProductUpdatedConsumer>();
            x.AddConsumer<ProductDeletedConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        serviceCollection.AddScoped<IProductService, ProductService>();
        return serviceCollection;
    }
}