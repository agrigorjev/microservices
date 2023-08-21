
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mandara.CalendarsService.GrpcServices;
using Xunit;
using Xunit.Abstractions;
using Mandara.CalendarsService.Services;
using Mandara.CalendarsService.Data;
using Mandara.CalendarsGrpcTests.Service;
using Mandara.CalendarsGrpcTests.Helpers;
using static Mandara.CalendarsService.GrpcDefinitions.CalendarService;
using Mandara.CalendarsGrpcTests.IntegrationTests;
using Optional;
using Mandara.CalendarsService.GrpcDefinitions;

namespace Tests.Server.IntegrationTests
{
    public class MockedMandaraCalendarsTests : IntegrationTestBase
    {

        private static Random r = new Random(DateTime.Now.Millisecond);

        private static List<StockCalendar> testData;

        public MockedMandaraCalendarsTests(GrpcTestFixture<Startup> fixture,
            ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

            testData = generateList(100, 10, 5);
            var mockCalendar = new Mock<ICalendarsStorage>();

            mockCalendar.Setup(c => c.GetStockCalendars()).Returns(() =>
            {
                return testData;
            });

            mockCalendar.Setup(c => c.GetCalendarHolidays()).Returns(() =>
            {
                return testData.Where(sc=>sc.Holidays.Count>0).SelectMany(sc=>sc.Holidays).ToList();
            });

            mockCalendar.Setup(c => c.GetCalendarExpiryDates()).Returns(() =>
            {
                return testData.Where(sc => sc.FuturesExpiries.Count > 0).SelectMany(sc => sc.FuturesExpiries).ToList();
            });

            mockCalendar.Setup(c => c.GetStockCalendar(It.IsAny<int>())).Returns((int i)=>
            {
                StockCalendar stockCalendar = testData.First(sc => sc.CalendarId == i);
                return stockCalendar==null || stockCalendar.IsDefault() ? Option.None<StockCalendar>() : Option.Some(stockCalendar);
            });

            mockCalendar.Setup(c => c.GetCalendarHolidays(It.IsAny<int>())).Returns((int i) =>
            {
                StockCalendar stockCalendar = testData.First(sc => sc.CalendarId == i);
                return stockCalendar == null || stockCalendar.IsDefault() ? new List<CalendarHoliday>() : stockCalendar.Holidays.ToList();
            });

            mockCalendar.Setup(c => c.GetCalendarExpiryDates(It.IsAny<int>())).Returns((int i) =>
            {
                StockCalendar stockCalendar = testData.First(sc => sc.CalendarId == i);
                return stockCalendar == null || stockCalendar.IsDefault() ? new List<CalendarExpiryDate>() : stockCalendar.FuturesExpiries.ToList();
            });

            Fixture.ConfigureWebHost(builder =>
            {
                builder.ConfigureServices(
                    services => services.AddSingleton(mockCalendar.Object));
            });

        }

        [Fact]
        public void AllStockCalendars_Success()
        {
            // Arrange
            var client = new CalendarServiceClient(Channel);

            // Act
           var stockCalendarsGrpcMessage = client.GetAllStockCalendars(new Mandara.CalendarsService.GrpcDefinitions.GetAllRequestMessage());

            // Assert
            Assert.NotEmpty(stockCalendarsGrpcMessage.StockCalendars);
            Assert.Equal(testData.Count, stockCalendarsGrpcMessage.StockCalendars.Count);
            int rndId= r.Next(0, testData.Count - 1);
            Assert.True(stockEquals(testData[rndId], stockCalendarsGrpcMessage.StockCalendars[rndId]));
            rndId = r.Next(0, testData.Count - 1);
            Assert.True(stockEquals(testData[rndId], stockCalendarsGrpcMessage.StockCalendars[rndId]));
        }

        [Fact]
        public void SingleStockCalendar_Success()
        {
            // Arrange
            var client = new CalendarServiceClient(Channel);

            // Act
            int checkId = r.Next(0, testData.Count -1);
            var stockCalendarGrpcResponse = client.GetStockCalendar(new GetByIdRequestMessage() { Id = testData[checkId].CalendarId });

            // Assert
            Assert.NotNull(stockCalendarGrpcResponse.StockCalendarData);
            Assert.True(stockEquals(testData[checkId], stockCalendarGrpcResponse.StockCalendarData));
        }

        [Fact]
        public void AllHollydays_Success()
        {
            int v = testData.Aggregate(0, (i, sc2) => i + sc2.Holidays.Count);
            // Arrange
            var client = new CalendarServiceClient(Channel);

            // Act
            var holidaysGrpcMessage = client.GetAllHolidays(new GetAllRequestMessage());

            // Assert
            Assert.NotEmpty(holidaysGrpcMessage.Holidays);

            Assert.Equal(v, holidaysGrpcMessage.Holidays.Count);
            int rndId = r.Next(0, holidaysGrpcMessage.Holidays.Count - 1);
            StockCalendar victim = testData.First(sc => sc.CalendarId == holidaysGrpcMessage.Holidays[rndId].CalendarId);
            Assert.Contains(victim.Holidays, h => holidayEquals(h, holidaysGrpcMessage.Holidays[rndId]));
          
        }
        [Fact]
        public void SingleHoliday_Success()
        {
            // Arrange
            var client = new CalendarServiceClient(Channel);

            List<StockCalendar> wHolidays
                = testData.Where(sc => sc.Holidays.Count > 0).ToList();

            // Act
            int checkId = wHolidays[r.Next(0, wHolidays.Count - 1)].CalendarId;
            var holidayGrpcResponse = client.GetHolidays(new GetByIdRequestMessage() { Id = checkId });

            // Assert
            StockCalendar victim = testData.First(sc => sc.CalendarId == checkId);
            
            Assert.NotNull(holidayGrpcResponse.Holidays);

            int rndId = r.Next(0, holidayGrpcResponse.Holidays.Count - 1);
            Assert.Contains(victim.Holidays, h => holidayEquals(h, holidayGrpcResponse.Holidays[rndId]));
        }

        [Fact]
        public void AllExpiries_Success()
        {
            int v = testData.Aggregate(0, (i, sc2) => i + sc2.FuturesExpiries.Count);
            // Arrange
            var client = new CalendarServiceClient(Channel);

            // Act
            var expiryGrpcMessage = client.GetAllExpiryDates(new GetAllRequestMessage());

            // Assert
            Assert.NotEmpty(expiryGrpcMessage.ExpiryDates);
            int rndId = r.Next(0, testData.Count - 1);
            Assert.Equal(v, expiryGrpcMessage.ExpiryDates.Count);
            rndId = r.Next(0, expiryGrpcMessage.ExpiryDates.Count - 1);
            StockCalendar victim = testData.First(sc => sc.CalendarId == expiryGrpcMessage.ExpiryDates[rndId].CalendarId);
            Assert.Contains(victim.FuturesExpiries, h => futureExpiryEquals(h, expiryGrpcMessage.ExpiryDates[rndId]));

        }

        [Fact]
        public void SingleExpiry_Success()
        {
            // Arrange
            var client = new CalendarServiceClient(Channel);

            List<StockCalendar> wExpiries
                = testData.Where(sc => sc.FuturesExpiries.Count > 0).ToList();

            // Act
            int checkId = wExpiries[r.Next(0, wExpiries.Count - 1)].CalendarId;
            var expiryGrpcResponse = client.GetExpiryDates(new GetByIdRequestMessage() { Id = checkId });

            // Assert
            StockCalendar victim = testData.First(sc => sc.CalendarId == checkId);

            Assert.NotNull(expiryGrpcResponse.ExpiryDates);

            int rndId = r.Next(0, expiryGrpcResponse.ExpiryDates.Count - 1);
            Assert.Contains(victim.FuturesExpiries, h => futureExpiryEquals(h, expiryGrpcResponse.ExpiryDates[rndId]));
        }

        private DateTime randomDate(int minOffset)
        {
           
            return DateTime.Now.AddDays(r.Next(minOffset, minOffset+100));
        }

        private List<StockCalendar> generateList(int howManyCalendars, int limitHolidays,int limitExpires)
        {

           return Enumerable.Range(1, howManyCalendars).Select
                (x =>
                {
                    StockCalendar stockCalendar = new StockCalendar()
                    {
                        CalendarId = x,
                        CalendarType = (CalendarType)r.Next(0, 2),
                        Name = "calendat_" + x,
                        Correction = 0,
                        FuturesExpiries = new List<CalendarExpiryDate>(),
                        Holidays = new List<CalendarHoliday>(),
                        Timezone = TimeZoneInfo.Utc.DisplayName
                    };
                    if (stockCalendar.CalendarType == CalendarType.Holidays || stockCalendar.CalendarType == CalendarType.ExpiryAndHolidays)
                    {
                        stockCalendar.Holidays =
                        Enumerable.Range(1, r.Next(1, limitHolidays)).Select(h =>
                            new CalendarHoliday()
                            {
                                CalendarId = stockCalendar.CalendarId,
                                HolidayDate = randomDate(1)
                            }
                        ).ToList();

                    }
                    if (stockCalendar.CalendarType == CalendarType.Expiry || stockCalendar.CalendarType == CalendarType.ExpiryAndHolidays)
                    {
                        stockCalendar.FuturesExpiries =
                            Enumerable.Range(1, r.Next(1, limitExpires)).Select(e =>
                            new CalendarExpiryDate()
                            {
                                CalendarId = stockCalendar.CalendarId,
                                ExpiryDate = randomDate(1),
                                FuturesDate = randomDate(101)
                            }

                        ).ToList();

                    }
                    return stockCalendar;
                }).ToList();

        }


        private bool stockEquals(StockCalendar testDAO, StockCalendarGrpc recievd)
        {
            return
                testDAO.CalendarId == recievd.CalendarId &&
                testDAO.Holidays.Count == recievd.Holidays.Count &&
                testDAO.FuturesExpiries.Count == recievd.FutureExpiries.Count &&
                (int)testDAO.CalendarType == (int)recievd.CalendarType;
        }


        private bool holidayEquals(CalendarHoliday testDAO, HolidayGrpc recievd)
        {
            return
                testDAO.CalendarId == recievd.CalendarId &&
                testDAO.HolidayDate == recievd.HolidayDate.ToDateTime().ToLocalTime();

        }

        private bool futureExpiryEquals(CalendarExpiryDate testDAO,ExpiryDatesGrpc recievd)
        {
            return
                testDAO.CalendarId == recievd.CalendarId &&
                testDAO.ExpiryDate == recievd.ExpiryDate.ToDateTime().ToLocalTime() &&
                testDAO.FuturesDate == recievd.FutureDate.ToDateTime().ToLocalTime();

        }
    }
}