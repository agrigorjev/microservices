using Mandara.DataService.Configuration;
using Mandara.DataService.Data;
using Mandara.DataService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandara.DataService;

public static class ServiceCollectionExtension
{
    public static void AddDataStorages(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.Configure<DataStoragesSettings>(
            configurationManager.GetSection(DataStoragesSettings.SectionName));

        services.AddSingleton<IPortfoliosStorage, PortfoliosStorage>();

        services.AddDbContext<MandaraEntities>(options => {
            options.UseSqlServer(configurationManager.GetConnectionString("MandaraEntities"));
        }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

        services.AddDbContextFactory<MandaraEntities>(options => {
            options.UseSqlServer(configurationManager.GetConnectionString("MandaraEntities"));
        });
    }
}