using System.Threading.Tasks;

namespace HolidayOptimizer
{
    public interface IHolidayStatsCalculator
    {
        Task<string> GetCountryWithHighestHolidaysAmount(int year);

        Task<int?> GetMonthWithHighestHolidaysAmount(int year);

        Task<string> GetCountryWithHighestUniqueHolidaysAmount(int year);
    }
}