using System;
using Microsoft.Extensions.Caching.Memory;

namespace BlogApp.Services;

public class MemoryCachingService : IMemoryCachingService
{
    private readonly IMemoryCache memoryCache;

    public MemoryCachingService(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(string cacheKey,
    Func<Task<T>> retreiveDataFunc,
    TimeSpan? slidingExpiration = null)
    {
        T cacheData;
        if (!memoryCache.TryGetValue(cacheKey, out cacheData))
        {

            cacheData = await retreiveDataFunc();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromSeconds(10)
            };
            memoryCache.Set(cacheKey, cacheData, cacheEntryOptions);
        }

        return cacheData;
        
        
    }
}

public interface IMemoryCachingService
{
    Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> retreiveDataFunc, TimeSpan? slidingExpiration = null);
}
