using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Nager.Date.ApiGateway
{
    public class NagerDateCachingClient : INagerDateClient
    {
        public NagerDateCachingClient(INagerDateClient innerClient)
        {
            _innerClient = innerClient;
        }

        public async Task<IReadOnlyCollection<Holiday>> GetHolidays(string countryCode, int year)
        {
            var cacheKey = GetCacheKey(countryCode, year);
            if (_memoryCache.Get(cacheKey) is IReadOnlyCollection<Holiday> cachedValue)
            {
                return cachedValue;
            }

            var newValue = await _innerClient.GetHolidays(countryCode, year);
            _memoryCache.Set(cacheKey, newValue, GetExpirationDateForEntry(year));
            return newValue;
        }

        private DateTimeOffset GetExpirationDateForEntry(int year)
        {
            var now = DateTimeOffset.Now;
            if (now.Year >= year)
            {
                return DateTimeOffset.MaxValue;
            }

            return new DateTime(year, 1, 1).AddDays(-1);
        }

        private static string GetCacheKey(string countryCode, int year) => $"Cache-{countryCode}-{year}";


        private readonly MemoryCache _memoryCache = MemoryCache.Default;
        private readonly INagerDateClient _innerClient;
    }
}