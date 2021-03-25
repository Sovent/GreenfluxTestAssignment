using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nager.Date.ApiGateway
{
    public class NagerDateDelayingClient : INagerDateClient
    {
        public NagerDateDelayingClient(INagerDateClient innerClient, TimeSpan delay)
        {
            _innerClient = innerClient;
            _delay = delay;
        }
        
        public async Task<IReadOnlyCollection<Holiday>> GetHolidays(string countryCode, int year)
        {
            await Task.Delay(_delay);
            var result = await _innerClient.GetHolidays(countryCode, year);
            return result;
        }
        
        private readonly INagerDateClient _innerClient;
        private readonly TimeSpan _delay;
    }
}