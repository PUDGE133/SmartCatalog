using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace SmartCatalog.Infrastructure.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task<long> GetCounterAsync(string key, CancellationToken ct = default);
    Task<long> IncrementAsync(string key, CancellationToken ct = default);
}

public partial class RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private IDatabase Db => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var raw = await Db.StringGetAsync(key).WaitAsync(ct);
        if (raw.IsNullOrEmpty)
        {
            LogCacheMiss(key);
            return default;
        }

        LogCacheHit(key);
        return JsonSerializer.Deserialize<T>(raw.ToString(), JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var payload = JsonSerializer.Serialize(value, JsonOptions);
        var expiration = ttl.HasValue ? (Expiration)ttl.Value : Expiration.Default;

        await Db.StringSetAsync(key, payload, expiration).WaitAsync(ct);
        LogCacheSet(key, ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await Db.KeyDeleteAsync(key).WaitAsync(ct);
        LogCacheDel(key);
    }

    public async Task<long> GetCounterAsync(string key, CancellationToken ct = default)
    {
        var raw = await Db.StringGetAsync(key).WaitAsync(ct);
        return raw.IsNullOrEmpty ? 0 : (long)raw;
    }

    public Task<long> IncrementAsync(string key, CancellationToken ct = default)
    {
        return Db.StringIncrementAsync(key).WaitAsync(ct);
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