using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Date.ApiGateway
{
    public class NagerDateRateLimitingClient : INagerDateClient
    {
        public NagerDateRateLimitingClient(INagerDateClient innerClient, int maxConcurrentRequestsLimit)
        {
            _innerClient = innerClient;
            _throttler = new SemaphoreSlim(maxConcurrentRequestsLimit);
        }
        
        public async Task<IReadOnlyCollection<Holiday>> GetHolidays(string countryCode, int year)
        {
            await _throttler.WaitAsync();
            try
            {
                return await _innerClient.GetHolidays(countryCode, year);
            }
            finally
            {
                _throttler.Release();
            }
        }

        private readonly SemaphoreSlim _throttler;
        private readonly INagerDateClient _innerClient;
    }
}