using System.Net.Http;

namespace Nager.Date.ApiGateway
{
    public interface INagerDateClientFactory
    {
        INagerDateClient Create(HttpClient httpClient, NagerDateClientConfig config);
    }
}