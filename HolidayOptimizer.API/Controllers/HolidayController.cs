using System.Globalization;
using System.Threading.Tasks;
using HolidayOptimizer.Models;
using Microsoft.AspNetCore.Mvc;

namespace HolidayOptimizer.Controllers
{
    [ApiController]
    [Route("holidays/{year}")]
    public class HolidayController : ControllerBase
    {
        public HolidayController(IHolidayStatsCalculator holidayStatsCalculator)
        {
            _holidayStatsCalculator = holidayStatsCalculator;
        }
        
        [HttpGet]
        [Route("highest_amount_country")]
        public async Task<IActionResult> GetCountryWithHighestAmount(int year)
        {
            var countryCode = await _holidayStatsCalculator.GetCountryWithHighestHolidaysAmount(year);
            return countryCode == null 
                ? GetNotFoundResult(year) 
                : Ok(new Country { CountryCode = countryCode });
        }
        
        [HttpGet]
        [Route("highest_amount_month")]
        public async Task<IActionResult> GetMonthWithHighestAmount(int year)
        {
            var month = await _holidayStatsCalculator.GetMonthWithHighestHolidaysAmount(year);
            return month == null
                ? GetNotFoundResult(year)
                : Ok(new Month
                {
                    MonthNumber = month.Value,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Value)
                });
        }

        [HttpGet]
        [Route("highest_unique_amount_country")]
        public async Task<IActionResult> GetCountryWithHighestUniqueAmount(int year)
        {
            var countryCode = await _holidayStatsCalculator.GetCountryWithHighestUniqueHolidaysAmount(year);
            return countryCode == null 
                ? GetNotFoundResult(year) 
                : Ok(new Country {CountryCode = countryCode});
        }

        private IActionResult GetNotFoundResult(int year)
        {
            return NotFound($"No stats found for year {year}");
        }
        
        private readonly IHolidayStatsCalculator _holidayStatsCalculator;
    }
}