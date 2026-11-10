using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace SmartCatalog.Infrastructure.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task<long> IncrementVersionListsAsync(string key, CancellationToken ct = default);
    Task<long> GetOrAddCounterAsync(string key, long initialValue = 1, CancellationToken ct = default);
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

    public Task<long> IncrementVersionListsAsync(string key, CancellationToken ct = default)
    {
        return Db.StringIncrementAsync(key).WaitAsync(ct);
    }

    public async Task<long> GetOrAddCounterAsync(string key, long initialValue = 1, CancellationToken ct = default)
    {
        // 1. Поток пытается просто прочитать (самый частый сценарий)
        var raw = await Db.StringGetAsync(key).WaitAsync(ct);
        if (!raw.IsNullOrEmpty)
            return (long)raw;

        // 2. Если ключа нет, поток создаст его атомарно.
        // Если два потока вызовут это одновременно, только ОДИН получит true (создаст ключ).
        await Db.StringSetAsync(key, initialValue, when: When.NotExists).WaitAsync(ct);

        // 3. Перечитываем значение.
        // Если поток проиграл гонку, он прочитаем значение, установленное другим потоком.
        raw = await Db.StringGetAsync(key).WaitAsync(ct);

        return (long)raw;
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