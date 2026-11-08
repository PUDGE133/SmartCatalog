using Microsoft.EntityFrameworkCore;
using SmartCatalog.Infrastructure.Entities;
using SmartCatalog.Infrastructure.Enums;

namespace SmartCatalog.Infrastructure.Extensions;

internal static class QueryExtensions
{
    internal static IQueryable<Product> BuildQueryBase(this IQueryable<Product> query, Tracking tracking)
    {
        if (tracking == Tracking.No)
            query = query.AsNoTracking();

        return query;
    }
}