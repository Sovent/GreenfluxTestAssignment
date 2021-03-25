using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Nager.Date.ApiGateway;
using NUnit.Framework;

namespace HolidayOptimizer.API.Tests
{
    public class HolidayStatsCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
            _dateClientMock = new Mock<INagerDateClient>();
        }

        [Test]
        public async Task GetCountryWithHighestHolidaysAmountWhenThereAreNoHolidaysForRequestedYear_ReturnsNull()
        {
            SetupHolidaysForCountry(NetherlandsCountryCode, Array.Empty<Holiday>());
            var calculator = GetCalculator(NetherlandsCountryCode);

            var result = await calculator.GetCountryWithHighestHolidaysAmount(2021);
            
            Assert.IsNull(result);
        }
        
        [Test]
        public async Task GetMonthWithHighestHolidaysAmountWhenThereAreNoHolidaysForRequestedYear_ReturnsNull()
        {
            SetupHolidaysForCountry(NetherlandsCountryCode, Array.Empty<Holiday>());
            var calculator = GetCalculator(NetherlandsCountryCode);

            var result = await calculator.GetMonthWithHighestHolidaysAmount(2021);
            
            Assert.IsFalse(result.HasValue);
        }
        
        [Test]
        public async Task GetCountryWithHighestUniqueHolidaysAmountWhenThereAreNoHolidaysForRequestedYear_ReturnsNull()
        {
            SetupHolidaysForCountry(NetherlandsCountryCode, Array.Empty<Holiday>());
            var calculator = GetCalculator(NetherlandsCountryCode);

            var result = await calculator.GetCountryWithHighestUniqueHolidaysAmount(2021);
            
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCountryWithHighestUniqueHolidaysAmount_ReturnsProperCountry()
        {
            SetupHolidaysForCountry(NetherlandsCountryCode, new Holiday(), new Holiday());
            SetupHolidaysForCountry(RussiaCountryCode, new Holiday(), new Holiday(), new Holiday());
            SetupHolidaysForCountry(SpainCountryCode, Array.Empty<Holiday>());
            var calculator = GetCalculator(NetherlandsCountryCode, RussiaCountryCode, SpainCountryCode);

            var result = await calculator.GetCountryWithHighestHolidaysAmount(2021);
            
            Assert.AreEqual(RussiaCountryCode, result);
        }
        
        [Test]
        public async Task GetMonthWithHighestHolidaysAmount_ReturnsProperMonth()
        {
            const int aprilMonthNumber = 4;
            var aprilHoliday = new Holiday {Date = new DateTime(2021, aprilMonthNumber, 15)};
            var januaryHoliday = new Holiday {Date = new DateTime(2021, 1, 5)};
            var octoberHoliday = new Holiday {Date = new DateTime(2021, 10, 25)};
            SetupHolidaysForCountry(NetherlandsCountryCode, aprilHoliday, januaryHoliday, octoberHoliday);
            SetupHolidaysForCountry(RussiaCountryCode, aprilHoliday, octoberHoliday);
            SetupHolidaysForCountry(SpainCountryCode, aprilHoliday);
            var calculator = GetCalculator(NetherlandsCountryCode, RussiaCountryCode, SpainCountryCode);

            var result = await calculator.GetMonthWithHighestHolidaysAmount(2021);
            
            Assert.AreEqual(aprilMonthNumber, result);
        }
        
        [Test]
        public async Task GetCountryWithHighestUniqueHolidaysAmount_ReturnsProperMonth()
        {
            var firstUniqueSpanishHoliday = new Holiday {Date = new DateTime(2021, 11, 12)};
            var secondUniqueSpanishHoliday = new Holiday {Date = new DateTime(2021, 2, 5)};
            var uniqueRussianHoliday = new Holiday {Date = new DateTime(2021, 5, 1)};
            var newYear = new Holiday {Date = new DateTime(2021, 1, 1)};
            var womenDay = new Holiday {Date = new DateTime(2021, 3, 8)};
            SetupHolidaysForCountry(NetherlandsCountryCode, newYear, womenDay);
            SetupHolidaysForCountry(RussiaCountryCode, newYear, womenDay, uniqueRussianHoliday);
            SetupHolidaysForCountry(SpainCountryCode, firstUniqueSpanishHoliday, secondUniqueSpanishHoliday);
            var calculator = GetCalculator(NetherlandsCountryCode, RussiaCountryCode, SpainCountryCode);

            var result = await calculator.GetCountryWithHighestUniqueHolidaysAmount(2021);
            
            Assert.AreEqual(SpainCountryCode, result);
        }
        
        private HolidayStatsCalculator GetCalculator(params string[] countryCodes)
        {
            return new HolidayStatsCalculator(_dateClientMock.Object, countryCodes);
        }

        private void SetupHolidaysForCountry(string countryCode, params Holiday[] holidays)
        {
            _dateClientMock
                .Setup(client => client.GetHolidays(countryCode, It.IsAny<int>()))
                .Returns(() => Task.FromResult((IReadOnlyCollection<Holiday>)holidays));
        }

        private const string RussiaCountryCode = "ru";
        private const string NetherlandsCountryCode = "nl";
        private const string SpainCountryCode = "es";
        
        private Mock<INagerDateClient> _dateClientMock;
    }
}