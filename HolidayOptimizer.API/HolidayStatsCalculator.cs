using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nager.Date.ApiGateway;

namespace HolidayOptimizer
{
    public class HolidayStatsCalculator : IHolidayStatsCalculator
    {
        public HolidayStatsCalculator(INagerDateClient nagerDateClient, string[] supportedCountryCodes)
        {
            _nagerDateClient = nagerDateClient;
            _supportedCountryCodes = supportedCountryCodes;
        }
        
        public async Task<string> GetCountryWithHighestHolidaysAmount(int year)
        {
            var countriesHolidays = await GetCountriesHolidays(year);
            var countryHolidays = countriesHolidays
                .Where(holidays => holidays.Value.Any())
                .OrderByDescending(holidays => holidays.Value.Count)
                .FirstOrDefault();
            if (countryHolidays.Equals(default(KeyValuePair<string, IReadOnlyCollection<Holiday>>)))
            {
                return null;
            }
            
            return countryHolidays.Key;
        }

        public async Task<int?> GetMonthWithHighestHolidaysAmount(int year)
        {
            var countriesHolidays = await GetCountriesHolidays(year);
            return countriesHolidays.Values
                .SelectMany(holidays => holidays.Select(holiday => holiday.Date.Month))
                .GroupBy(month => month)
                .OrderByDescending(grouping => grouping.Count())
                .FirstOrDefault()
                ?.Key;
        }

        public async Task<string> GetCountryWithHighestUniqueHolidaysAmount(int year)
        {
            var countriesHolidays = await GetCountriesHolidays(year);
            var allHolidays = countriesHolidays.SelectMany(holidays =>
                holidays.Value.Select(holidayInfo => (Info: holidayInfo, Country: holidays.Key)));
            var uniqueHolidays = allHolidays
                .GroupBy(holiday => holiday.Info.Date.Date)
                .Where(grouping => grouping.Count() == 1)
                .Select(grouping => grouping.Single());
            return uniqueHolidays
                .GroupBy(holiday => holiday.Country)
                .OrderByDescending(grouping => grouping.Count())
                .FirstOrDefault()
                ?.Key;
        }

        private async Task<IReadOnlyDictionary<string, IReadOnlyCollection<Holiday>>> GetCountriesHolidays(int year)
        {
            var countriesHolidays = new Dictionary<string, IReadOnlyCollection<Holiday>>();
            foreach (var countryCode in _supportedCountryCodes)
            {
                var holidays = await _nagerDateClient.GetHolidays(countryCode, year);
                countriesHolidays[countryCode] = holidays;
            }

            return countriesHolidays;
        }
        
        private readonly INagerDateClient _nagerDateClient;
        private readonly string[] _supportedCountryCodes;
    }
}