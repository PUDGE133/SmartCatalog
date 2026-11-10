using MassTransit;
using SmartCatalog.BusinessLogic.Consumers.Invalidators;
using SmartCatalog.Contracts.Events;

namespace SmartCatalog.BusinessLogic.Consumers;

public class ProductCreatedConsumer(ProductCacheInvalidator invalidator) : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        return invalidator.InvalidateProductListsAsync(context.CancellationToken);
    }
}

public class ProductUpdatedConsumer(ProductCacheInvalidator invalidator) : IConsumer<ProductUpdatedEvent>
{
    public Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        return invalidator.InvalidateAllAsync(context.Message.ProductId, context.CancellationToken);
    }
}

public class ProductDeletedConsumer(ProductCacheInvalidator invalidator) : IConsumer<ProductDeletedEvent>
{
    public Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        return invalidator.InvalidateAllAsync(context.Message.ProductId, context.CancellationToken);
    }
}