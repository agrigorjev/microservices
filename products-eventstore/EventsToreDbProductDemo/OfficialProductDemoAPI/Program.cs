using Google.Api;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using OfficialProductDemoAPI.Services;
using OfficialProductDemoAPI.Services.Cache;
using OfficialProductDemoAPI.Services.Contracts;

var logger = LogManager.GetCurrentClassLogger();
logger.Debug("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddGrpc();
    builder.Services.AddGrpcSwagger();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
    });

    builder.Services.AddEventStoreClient(builder.Configuration.GetSection("EventsStrore").GetValue<String>("local"));

    builder.Services.AddSingleton<CacheService<OfficialProduct>, OfficialProductsCacheService>();
    builder.Services.AddSingleton<CacheService<Currency>, CurrencyCacheService>();
    builder.Services.AddSingleton<CacheService<Unit>, UnitCacheService>();
    builder.Services.AddSingleton<CacheService<Region>, RegionCacheService>();
    builder.Services.AddSingleton<INotifyService<ServiceEventMessage>, ServiceNotification>();
    builder.Services.AddHostedService<OperationalService>();

    var app = builder.Build();


    app.MapGrpcService<OfficialProductGrpcService>();

    app.Run();
}
catch(Exception ex)
{
    logger.Error("Application run throw exception",ex);
}
