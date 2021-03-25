using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nager.Date.ApiGateway
{
    public interface INagerDateClient
    {
        Task<IReadOnlyCollection<Holiday>> GetHolidays(string countryCode, int year);
    }
}