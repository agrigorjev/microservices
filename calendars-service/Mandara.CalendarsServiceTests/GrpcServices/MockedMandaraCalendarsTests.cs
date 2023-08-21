
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

namespace Tests.Server.IntegrationTests
{
    public class MockedMandaraCalendarsTests : IntegrationTestBase
    {
    

        public MockedMandaraCalendarsTests(GrpcTestFixture<Startup> fixture,
            ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var mockCalendar = new Mock<ICalendarsStorage>();
            mockCalendar.Setup(c => c.GetStockCalendars()).Returns(() =>
            {
                return new List<StockCalendar>();
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
            Assert.Empty(stockCalendarsGrpcMessage.StockCalendars);
        }


       
    }
}