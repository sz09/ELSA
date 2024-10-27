using ELSA.DAL.Models.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;

namespace ELSA.CodeChallenge.Repositories.Caching
{

    public class Queue<T>
    {
        private static readonly Lazy<AsyncQueue<Func<Task<T>>>> _queue = new Lazy<AsyncQueue<Func<Task<T>>>>(() =>
        {
            return new AsyncQueue<Func<Task<T>>>();
        });

        public static AsyncQueue<Func<Task<T>>> Instance => _queue.Value;
    }

    public class InMemoryCache
    {
        private static readonly ConcurrentDictionary<string, MemoryCache> _caches = new();
        private readonly IOptions<MemoryCacheOptions> _optionsAccessor;
        private readonly TimeSpan _cacheItemExpireAfter;
        public InMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, TimeSpan cacheItemExpireAfter)
        {
            _optionsAccessor = optionsAccessor;
            _cacheItemExpireAfter = cacheItemExpireAfter;
        }
        
        public async Task<T> GetOrCreateAsync<T>(string group, string itemKey, Func<Task<T>> factory) where T : BaseEntity
        {
            var cache = GetOrCreateCache(group);
            if (!cache.TryGetValue(itemKey, out T cachedItem))
            {
                // Lock to ensure only one thread fetches the item
                var lockKey = $"{group}-{itemKey}";
                var lockObject = _caches.GetOrAdd(lockKey, _ => new MemoryCache(_optionsAccessor));

                try
                {
                    // Check again after acquiring the lock
                    if (!cache.TryGetValue(itemKey, out cachedItem))
                    {
                        cachedItem = await factory();
                        if (cachedItem != null)
                        {
                            cache.Set(itemKey, cachedItem, DateTimeOffset.Now.Add(_cacheItemExpireAfter));
                        }
                    }
                }
                finally
                {
                    // Clean up lock object
                    _caches.TryRemove(lockKey, out _);
                }
            }

            return cachedItem;
        }
        public MemoryCache GetOrCreateCache(string cacheKey)
        {
            return _caches.GetOrAdd(cacheKey, _ => new MemoryCache(_optionsAccessor));
        }

        public void InvalidateCache(string group)
        {
            if(_caches.TryRemove(group, out var memoryCache))
            {
                memoryCache.Dispose();
            }
        }
    }
}
