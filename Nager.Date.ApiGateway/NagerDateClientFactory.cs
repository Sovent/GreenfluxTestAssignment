using System;
using System.Net.Http;

namespace Nager.Date.ApiGateway
{
    public class NagerDateClientFactory : INagerDateClientFactory
    {
        public INagerDateClient Create(HttpClient httpClient, NagerDateClientConfig config)
        {
            httpClient.BaseAddress = new Uri(config.ApiAddress);
            INagerDateClient client = new NagerDateClient(httpClient);

            if (config.RequestsDelay.HasValue)
            {
                client = new NagerDateDelayingClient(client, config.RequestsDelay.Value);
            }

            if (config.MaxConcurrentRequestsLimit.HasValue)
            {
                client = new NagerDateRateLimitingClient(client, config.MaxConcurrentRequestsLimit.Value);
            }
            
            if (config.UseCache)
            {
                client = new NagerDateCachingClient(client);
            }

            return client;
        }
    }
}