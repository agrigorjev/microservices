using Mandara.TradeApiService.Configuration;
using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.GrpcServices;
using Mandara.TradeApiService.Repositories;
using Mandara.TradeApiService.Repositories.Contracts;
using Mandara.TradeApiService.Services;
using Mandara.TradeApiService.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using static Mandara.TradeApiService.GrpcDefinitions.TradeApiGrpcService;
using Assert = Xunit.Assert;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Mandara.TradeApiService.UnitTests.GrpcServices;

[TestClass]
public class TradeApiGrpcServiceTests : IntegrationTestBase
{
    private readonly Mock<IOptions<DataStoragesSettings>> _settings = new();
    private readonly Mock<IDbContextFactory<MandaraEntities>> _contextFactory = new();
    private readonly Mock<ITradesRepository> _tradesRepository= new();

    [TestInitialize]
    public void Initialise()
    {
    }

    private DataStorage CreateStorage()
    {
        return new DataStorage(_settings.Object, _contextFactory.Object);
    }

    public TradeApiGrpcServiceTests(GrpcTestFixture<Startup> fixture,
    ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        TestDbInitializer.Init();
        _contextFactory
            .Setup(x => x.CreateDbContext())
            .Returns(() => { return TestDbInitializer.CreateContext(); });

        Fixture.ConfigureWebHost(builder =>
        {
            var config = new ConfigurationBuilder().Build();
            builder.ConfigureServices(
                services => { 
                    services.AddSingleton<IDbContextFactory<MandaraEntities>>(_contextFactory.Object);
                    services.AddSingleton<IDataStorage, DataStorage>();
                    services.AddSingleton<ITradesRepository, TradesRepository>();
                    services.Configure<ServiceSettings>(config);
                });

        });
    }

    [Fact]
    public void TestGetTradeAddPrerequisitesFakedTradeCaptureRequest()
    {
        // Arrange
        var client = new TradeApiGrpcServiceClient(Channel);

        // Act
        var requestMessage = new Mandara.TradeApiService.GrpcDefinitions.TradeAddPrerequisitesRequestGrpcMessage()
        {
            UserId = 1,
            TradeCaptureId = 23
        };
        var tradeAddPrerequisitesResponseGrpcMessage = client.GetTradeAddPrerequisites(requestMessage);

        // Assert
        Assert.NotEmpty(tradeAddPrerequisitesResponseGrpcMessage.Units);
        Assert.Contains("Unable to locate", tradeAddPrerequisitesResponseGrpcMessage.ErrorMessage);
    }

    [Fact]
    public void TestGetTradeAddPrerequisitesEmptyRequest()
    {
        // Arrange
        var client = new TradeApiGrpcServiceClient(Channel);

        // Act
        var requestMessage = new Mandara.TradeApiService.GrpcDefinitions.TradeAddPrerequisitesRequestGrpcMessage()
        {
        };
        var tradeAddPrerequisitesResponseGrpcMessage = client.GetTradeAddPrerequisites(requestMessage);

        // Assert
        Assert.NotEmpty(tradeAddPrerequisitesResponseGrpcMessage.Units);
        Assert.DoesNotContain("Unable to locate", tradeAddPrerequisitesResponseGrpcMessage.ErrorMessage);
        Assert.Empty(tradeAddPrerequisitesResponseGrpcMessage.ErrorMessage);
    }
}
