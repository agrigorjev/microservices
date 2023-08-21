using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.GrpcServices;
using Mandara.TradeApiService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.TradeApiService.UnitTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(o => o.EnableDetailedErrors = true);
        services.AddSingleton<IDataStorage, DataStorage>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<TradeApiGrpcService>();
        });
    }
}
