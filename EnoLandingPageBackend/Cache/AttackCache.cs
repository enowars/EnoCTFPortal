
namespace EnoLandingPageBackend.Cache
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    public class CustomMemoryCache<T>
    {
        private readonly string defaultKey = "default";
        private MemoryCache _cache { get; set; }
        public CustomMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public T TryGetDefault()
        {
            T cacheEntry;
            this._cache.TryGetValue(defaultKey, out cacheEntry);
            return cacheEntry;
        }
        public void InvalidateDefault()
        {
            this._cache.Remove(defaultKey);
        }

        public void CreateDefault(T attackInfo)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1);
            _cache.Set(this.defaultKey, attackInfo, cacheEntryOptions);
        }

        public T GetOrCreate(object key, Func<T> createItem)
        {
            T cacheEntry;
            if (!_cache.TryGetValue(key, out cacheEntry))// Look for cache key.
            {
                cacheEntry = createItem();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public async Task<T> GetOrCreateAsync(object key, Func<Task<T>> createItem)
        {
            var func = await createItem();
            return this.GetOrCreate(key, () =>
            {
                return func;
            });
        }
    }
}