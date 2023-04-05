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
        IHostApplicationLifetime _hostApplicationLifetime;

        public DumpUploadService(IHostApplicationLifetime hostApplicationLifetime, IDbContextFactory<MandaraEntities> contextFactory, EventStoreClient client)
        {
            _contextFactory= contextFactory;
            _client= client;
            _hostApplicationLifetime   = hostApplicationLifetime;

        }


      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
              

                using (MandaraEntities productsDb = _contextFactory.CreateDbContext())
                {

                    var curencyCollection = productsDb.Currencies.ToDictionary(c => c.CurrencyId);
                    var regionCollection = productsDb.Regions.ToDictionary(c => c.RegionId);
                    var unitCollection = productsDb.Units.ToDictionary(c => c.UnitId);

                    int chunksC = 0;
                    await curencyCollection.Values
                        .Select(c => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Currency>(c))))
                        .Chunk(10).
                         ToObservable<EventData[]>()
                        .ForEachAsync(evts => _client.AppendToStreamAsync("Currency.v1", StreamState.Any, evts).ContinueWith(res =>
                         Console.WriteLine("Currency: {0} {1}", chunksC++, res.Result.LogPosition)));
                    
                    int chunkR = 0;
                    await regionCollection.Values
                        .Select(c => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Region>(c))))
                        .Chunk(10).
                         ToObservable<EventData[]>()
                        .ForEachAsync(evts => _client.AppendToStreamAsync("Region.v1", StreamState.Any, evts).ContinueWith(res =>
                         Console.WriteLine("Region: {0} {1}", chunkR++, res.Result.LogPosition)));

                    int chunkU= 0;
                    await unitCollection.Values
                       .Select(c => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Unit>(c))))
                       .Chunk(10).
                        ToObservable<EventData[]>()
                       .ForEachAsync(evts => _client.AppendToStreamAsync("Region.v1", StreamState.Any, evts).ContinueWith(res =>
                        Console.WriteLine("Unit: {0} {1}", chunkU++, res.Result.LogPosition)));

                    var opList = productsDb.OfficialProducts.ToList();

                    int chunkP = 0;
                    opList.ForEach(op =>
                    {
                        op.UnitGuid = unitCollection[op.PriceUnitId].Id;
                        op.CurrencyGuId = curencyCollection[op.CurrencyId].Id;
                        if (op.RegionId.HasValue) op.RegionGuId = regionCollection[op.RegionId.Value].Id;
                    });

                   await opList
                     .Select(op => new EventData(Uuid.FromGuid(Guid.NewGuid()), "new", Encoding.UTF8.GetBytes(JsonSerializer.Serialize<OfficialProduct>(op))))
                    .Chunk(10)
                      .ToObservable<EventData[]>()
                       .ForEachAsync(evts => _client.AppendToStreamAsync("OfficialProduct.v1", StreamState.Any, evts).ContinueWith(res =>
                        Console.WriteLine("OP: {0} {1}", chunkP++, res.Result.LogPosition)));
                }
            }
            finally
            {
                 await _client.DisposeAsync();  
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
