using Mandara.DataService.BackgroundServices;
using Mandara.DataService.Data;
using Mandara.DataService.GrpcServices;
using Mandara.DataService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

var azAppConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");

if (!string.IsNullOrWhiteSpace(azAppConfigConnectionString))
    builder.Configuration.AddAzureAppConfiguration(azAppConfigConnectionString);

//builder.Services.AddGrpc();
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
builder.Services.AddSingleton<IPortfoliosStorage, PortfoliosStorage>();

builder.Services.AddHostedService<PortfoliosReaderService>();

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

app.MapGrpcService<PortfoliosGrpcService>();

app.Run();
