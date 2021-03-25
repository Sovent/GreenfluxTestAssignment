using System;

namespace Nager.Date.ApiGateway
{
    public class NagerDateClientConfig
    {
        public string ApiAddress { get; set; } = "https://date.nager.at";
        
        public bool UseCache { get; set; }
        
        public int? MaxConcurrentRequestsLimit { get; set; }
        
        public TimeSpan? RequestsDelay { get; set; }
    }
}