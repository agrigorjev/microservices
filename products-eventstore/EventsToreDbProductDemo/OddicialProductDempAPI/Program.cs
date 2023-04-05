using Google.Api;
using MandaraDemoDTO;
using Microsoft.OpenApi.Models;
using OfficialProductDemoAPI.Services;
using OfficialProductDemoAPI.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});

builder.Services.AddEventStoreClient(builder.Configuration.GetSection("EventsStrore").GetValue<String>("local"));
builder.Services.AddSingleton<IDataService<OfficialProduct>,CacheService>();
builder.Services.AddHostedService<OperationalService>();

var app = builder.Build();

app.MapGrpcService<OfficialProductGrpcService>();

app.Run();
