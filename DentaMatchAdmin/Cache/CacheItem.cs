using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentaMatchAdmin.Cache
{
    public class CacheItem
    {
        private readonly IMemoryCache _cache;
        public CacheItem(IMemoryCache cache)
        {
            _cache = cache;
        }
        public void storeInDays(string key, object value, int time)
        {
            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(time));
            _cache.Set(key, value, cacheOptions);
        }
        public object Retrieve(string key)
        {
            return _cache.Get(key);
        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
