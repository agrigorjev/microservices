using DataImport.DataEntries;
using DataImport.DataEntries.Contract;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataImport.Services
{
    internal sealed class DumpUploadService : BackgroundService
    {

        IDbContextFactory<MandaraEntities> _contextFactory;
        EventStoreClient _client;

        public DumpUploadService( IDbContextFactory<MandaraEntities> contextFactory, EventStoreClient client)
        {
            _contextFactory= contextFactory;
            _client= client;
        }
       

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            int chunks = 0;

            using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
            {
              
                var productTasks=productsDb.OfficialProducts
                .AsEnumerable()
                .Select(op => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<OfficialProduct>(op))))
                .Chunk(10)
                .Select(evts => _client.AppendToStreamAsync("OfficialProducts", StreamState.Any, evts).ContinueWith(res =>
                {
                    Console.WriteLine("OP: {0} {1}", chunks++, res.Result.LogPosition);
                })               
                ).ToList();

                var unitTasks = productsDb.Units
              .AsEnumerable()
              .Select(op => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Unit>(op))))
              .Chunk(10)
              .Select(evts => _client.AppendToStreamAsync("Units", StreamState.Any, evts).ContinueWith(res =>
              {
                  Console.WriteLine("Units: {0} {1}", chunks++, res.Result.LogPosition);
              })
              ).ToList();

                var regionTasks = productsDb.Regions
              .AsEnumerable()
              .Select(op => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Region>(op))))
              .Chunk(10)
              .Select(evts => _client.AppendToStreamAsync("Regions", StreamState.Any, evts).ContinueWith(res =>
              {
                  Console.WriteLine("Region : {0} {1}", chunks++, res.Result.LogPosition);
              })
              ).ToList();

                var currencyTasks = productsDb.Currencies
              .AsEnumerable()
              .Select(op => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Currency>(op))))
              .Chunk(10)
              .Select(evts => _client.AppendToStreamAsync("Currencies", StreamState.Any, evts).ContinueWith(res =>
              {
                  Console.WriteLine("Carrency: {0} {1}", chunks++, res.Result.LogPosition);
              })
              ).ToList();

                Task.WaitAll(productTasks.Concat(currencyTasks).Concat(regionTasks).Concat(unitTasks).ToArray());
            }
            return Task.CompletedTask;
        }
    }
}
