using Mandara.ProductService.Configuration;
using Mandara.ProductService.Data;
using Mandara.ProductService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandara.CalendarsService;

public static class ServiceCollectionExtension
{
    public static void AddDataStorages(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.Configure<DataStoragesSettings>(
            configurationManager.GetSection(DataStoragesSettings.SectionName));

        services.AddSingleton<IProductStorage, ProductStorage>();

        services.AddDbContext<MandaraEntities>(options => {
            options.UseSqlServer(configurationManager.GetConnectionString("MandaraEntities"));
        }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

        services.AddDbContextFactory<MandaraEntities>(options => {
            options.UseSqlServer(configurationManager.GetConnectionString("MandaraEntities"));
        });
    }
}