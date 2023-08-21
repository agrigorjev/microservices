using DataImport.DataEntries;
using DataImport.Services;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder(args);


builder.ConfigureServices((context, services) =>
 {
     var eventStoreConnection = context.Configuration.GetSection("EventsStrore").GetValue<String>("local");
     services.AddDbContext<MandaraEntities>(options => {
         options.UseSqlServer(context.Configuration.GetConnectionString("MandaraEntities"));
     }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);

     services.AddDbContextFactory<MandaraEntities>(options => {
         options.UseSqlServer(context.Configuration.GetConnectionString("MandaraEntities"));
     });
     services.AddEventStoreClient(eventStoreConnection);
     services.AddHostedService<DumpUploadService>();


 });
using IHost host = builder.Build();
host.RunAsync();
