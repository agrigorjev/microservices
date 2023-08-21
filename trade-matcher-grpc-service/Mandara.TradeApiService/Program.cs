using Mandara.TradeApiService.BackgroundServices;
using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.Data;
//using Mandara.TradeMatcherGRPCService.GrpcServices;
using Mandara.TradeApiService.Services;
using Mandara.TradeApiService.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.X509Certificates;
using Mandara.TradeApiService.Repositories.Contracts;
using Mandara.TradeApiService.Repositories;
using Mandara.TradeApiService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

var azAppConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");

if (!string.IsNullOrWhiteSpace(azAppConfigConnectionString))
    builder.Configuration.AddAzureAppConfiguration(azAppConfigConnectionString);

builder.Services.AddGrpc();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});
builder.Services.AddDbContext<MandaraEntities>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MandaraEntities"));
}, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<MandaraEntities>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MandaraEntities"));
});
builder.Services.AddSingleton<IDataStorage, DataStorage>();
builder.Services.AddSingleton<ITradesRepository, TradesRepository>();

builder.Services.Configure<ServiceSettings>(
    builder.Configuration.GetSection(ServiceSettings.SectionName));

builder.Services.AddHostedService<PeriodicallyReaderService>();

var certificatePassword = builder.Configuration["ServerCertificatePassword"];
builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.ConfigureHttpsDefaults(https =>
    {
        https.ServerCertificate = new X509Certificate2("server-certificate.pfx", certificatePassword);
    });
});


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.MapGrpcService<TradeApiGrpcService>();

app.Run();