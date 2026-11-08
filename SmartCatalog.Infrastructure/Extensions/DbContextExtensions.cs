using Microsoft.EntityFrameworkCore;

namespace SmartCatalog.Infrastructure.Extensions;

internal static class DbContextExtensions
{
    public static void SetOriginalVersion<TEntity>(this DbContext context, TEntity entity, string propertyName, uint version)
        where TEntity : class
    {
        context.Entry(entity).OriginalValues[propertyName] = version;
    }
}