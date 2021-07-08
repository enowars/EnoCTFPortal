using System;
using System.Threading.Tasks;
using EnoCore.AttackInfo;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EnoLandingPageBackend.Cache
{
    public class AttackCache
    {
        private readonly string defaultKey = "default";
        private MemoryCache _cache { get; set; }
        public AttackCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public AttackInfo TryGetDefault()
        {
            AttackInfo cacheEntry;
            this._cache.TryGetValue(defaultKey, out cacheEntry);
            return cacheEntry;
        }
        public void InvalidateDefault()
        {
            this._cache.Remove(defaultKey);
        }

        public void CreateDefault(AttackInfo attackInfo)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1);
            _cache.Set(this.defaultKey, attackInfo, cacheEntryOptions);
        }

        public AttackInfo GetOrCreate(object key, Func<AttackInfo> createItem)
        {
            AttackInfo cacheEntry;
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

        public async Task<AttackInfo> GetOrCreateAsync(object key, Func<Task<AttackInfo>> createItem)
        {
            var func = await createItem();
            return this.GetOrCreate(key, () =>
            {
                return func;
            });
        }
    }
}