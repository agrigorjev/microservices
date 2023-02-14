using Mandara.CalendarsService.GrpcServices;
using Mandara.CalendarsService.Services;
using Mandara.ProductService.GrpcDefinitions;
using Mandara.ProductService.GrpcServices;
using Mandara.ProductService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.CalendarsGrpcTests.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(o => o.EnableDetailedErrors = true);
            services.AddSingleton<ICalendarsStorage, CalendarsStorage>();
            services.AddSingleton<IProductStorage, ProductStorage>();
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
                endpoints.MapGrpcService<CalendarsGrpcService>();
                 endpoints.MapGrpcService<ProductsGrpcService>();
            });
        }
    }
}
