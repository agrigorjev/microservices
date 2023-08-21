using Mandara.ProductConfiguration.Contracts;
using Mandara.ProductConfiguration.Data;
using Mandara.ProductConfiguration.GrpcServices;
using Mandara.ProductConfiguration.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var azAppConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");



//builder.Services.AddGrpc();
builder.Services.AddGrpc();
builder.Services.AddDbContext<MandaraEntities>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MandaraEntities"));
}, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<MandaraEntities>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MandaraEntities"));
});
builder.Services.AddSingleton<IDataStorage, DataStorage>();

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


app.MapGrpcService<ProductsGrpcService>();

app.Run();