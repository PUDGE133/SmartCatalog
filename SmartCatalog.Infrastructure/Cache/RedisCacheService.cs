using Microsoft.Extensions.Logging;

namespace SmartCatalog.Infrastructure.Cache;

public partial class RedisCacheService(ILogger<RedisCacheService> logger) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {

    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {

    }
}

public partial class RedisCacheService : ICacheService
{
    [LoggerMessage(Level = LogLevel.Information, Message = "[Cache MISS] {Key}")]
    private partial void LogCacheMiss(string key);

    [LoggerMessage(Level = LogLevel.Information, Message = "[Cache HIT]  {Key}")]
    private partial void LogCacheHit(string key);

    [LoggerMessage(Level = LogLevel.Information, Message = "[Cache SET]  {Key} (ttl={Ttl})")]
    private partial void LogCacheSet(string key, TimeSpan? ttl);

    [LoggerMessage(Level = LogLevel.Information, Message = "[Cache DEL]  {Key}")]
    private partial void LogCacheDel(string key);
}