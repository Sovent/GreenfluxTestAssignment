using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Nager.Date.ApiGateway
{
    public class NagerDateClient : INagerDateClient
    {
        public NagerDateClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<IReadOnlyCollection<Holiday>> GetHolidays(string countryCode, int year)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync($"/api/v2/publicholidays/{year}/{countryCode}");
            }
            catch (Exception exception)
            {
                throw new NagerDateApiGatewayException("Request to Nagel Date API failed", exception);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new NagerDateApiGatewayException($"Nagel Date API responded with error: {errorMessage}");
            }

            try
            {
                var holidays = await response.Content.ReadFromJsonAsync<Holiday[]>();
                return holidays;
            }
            catch (Exception exception)
            {
                throw new NagerDateApiGatewayException("Error while deserializing response from API", exception);
            }
        }
        
        private readonly HttpClient _httpClient;
    }
}