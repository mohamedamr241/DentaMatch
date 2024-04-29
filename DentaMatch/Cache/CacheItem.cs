using DentaMatch.Models.Dental_Case.Comments;
using DentaMatch.ViewModel.Dental_Cases;
using Microsoft.Extensions.Caching.Memory;

namespace DentaMatch.Cache
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
        public void storeArrayInDays(string key, List<DentalCaseCommentVM> value, int time)
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
