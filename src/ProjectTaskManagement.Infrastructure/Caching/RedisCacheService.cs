using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProjectTaskManagement.Application.Common.Interfaces;

namespace ProjectTaskManagement.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var data = await _cache.GetStringAsync(key, cancellationToken);
        return data is null ? default : JsonSerializer.Deserialize<T>(data, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    public async Task<string> GetOrCreateVersionAsync(string versionKey, CancellationToken cancellationToken = default)
    {
        var version = await _cache.GetStringAsync(versionKey, cancellationToken);
        if (!string.IsNullOrEmpty(version))
            return version;

        version = Guid.NewGuid().ToString("N");
        await _cache.SetStringAsync(versionKey, version, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(12)
        }, cancellationToken);

        return version;
    }

    public async Task InvalidateVersionAsync(string versionKey, CancellationToken cancellationToken = default)
    {
        var newVersion = Guid.NewGuid().ToString("N");
        await _cache.SetStringAsync(versionKey, newVersion, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(12)
        }, cancellationToken);
    }
}
