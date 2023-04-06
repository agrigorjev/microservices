using EventStore.Client;
using Google.Api;
using MandaraDemoDTO;
using OfficialProductDemoAPI.Extensions;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Threading;
using static EventStore.Client.StreamMessage;

namespace OfficialProductDemoAPI.Services

{
    public class CacheService : IDataService<OfficialProduct> 
    { 


        private ConcurrentDictionary<Guid,OfficialProduct> _products=new ConcurrentDictionary<Guid, OfficialProduct>();
        private ConcurrentDictionary<Guid, Unit> _priceUnits = new ConcurrentDictionary<Guid, Unit>();
        private ConcurrentDictionary<Guid, Currency> _currencies = new ConcurrentDictionary<Guid, Currency>();
        private ConcurrentDictionary<Guid, Region> _regions = new ConcurrentDictionary<Guid, Region>();

    

        private CancellationToken _stopSubsciption=new CancellationToken();

        private EventStoreClient _client;
        public CacheService(EventStoreClient eventStoreClient){
            _client=eventStoreClient;
            
        }

        public void Dispose()
        {
           // _client.Dispose();
        }

        public List<OfficialProduct> GetList()
        {
            return _products.Values.ToList();

        }

        public Option<OfficialProduct> GetSingle(string id)
        {
            try
            {
                return _products.ContainsKey(Guid.Parse(id)) ? Option.Some<OfficialProduct>(_products[Guid.Parse(id)]) : Option.None<OfficialProduct>();
            }
            catch(Exception ex)
            {
                return Option.None<OfficialProduct>();
            }
        }

        private async Task LogEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
           await Console.Out.WriteLineAsync(String.Format("{0} [{1}],{2}", DateTime.Now,subscription.SubscriptionId,new String(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()))));
        }

        private async Task HandleCurrency(StreamSubscription subscription,ResolvedEvent evnt, CancellationToken cancellationToken)
        {

            await Task.Run(() => {
                try
                {
                    var currency = JsonSerializer.Deserialize<Currency>(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()));
                    if (evnt.Event.EventType != "delete")
                    {

                        if (currency != null)
                        {
                            _currencies.AddOrUpdate(currency.Id, currency, (key, oldValue) => currency);
                            _products.Where((r) => r.Value.Currency == currency)
                                .ForEach(p => p.Value.Currency = currency
                            );
                        }
                    }
                    else
                    {
                        if (_products.Values.All(p => p.CurrencyGuId != currency.Id))
                        {

                            if (_currencies.TryRemove(currency.Id, out Currency c))
                            {
                                Console.WriteLine("{0} removed", c.Id);
                            }
                            else
                            {
                                Console.WriteLine("{0} not removed", currency.Id);
                            }
                        }
                        else
                        {

                            Console.WriteLine("Consistency error {0} currency used", currency.Id);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }, cancellationToken).ContinueWith(t => LogEvent(subscription, evnt, cancellationToken));
           
        }

        private async Task HandleProduct(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
                try
                {
                    var officialProduct = JsonSerializer.Deserialize<OfficialProduct>(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()));
                    if (evnt.Event.EventType != "delete")
                    {

                        if (officialProduct != null)
                        {
                            _products.AddOrUpdate(officialProduct.Id, officialProduct, (key, oldValue) => officialProduct);
                        }
                    }
                    else
                    {
                      

                            if (_products.TryRemove(officialProduct.Id, out OfficialProduct c))
                            {
                                Console.WriteLine("{0} removed", officialProduct.Id);
                            }
                            else
                            {
                                Console.WriteLine("{0} not removed", officialProduct.Id);
                            }
                      
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }, cancellationToken).ContinueWith(t => LogEvent(subscription, evnt, cancellationToken));
        }

        private async Task HandlePriceUnit(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
            try
            {
                var priceUnit = JsonSerializer.Deserialize<Unit>(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()));
                if (evnt.Event.EventType != "delete")
                {

                    if (priceUnit != null)
                    {
                        _priceUnits.AddOrUpdate(priceUnit.Id, priceUnit, (key, oldValue) => priceUnit);
                        _products.Where((r) => r.Value.PriceUnit == priceUnit)
                        .ForEach(p => p.Value.PriceUnit = priceUnit
                            );
                      
                        }
                    }
                    else
                    {
                        if (_products.Values.All(p => p.UnitGuid != priceUnit.Id))
                        {

                            if (_priceUnits.TryRemove(priceUnit.Id, out Unit c))
                            {
                                Console.WriteLine("{0} removed", priceUnit.Id);
                            }
                            else
                            {
                                Console.WriteLine("{0} not removed", priceUnit.Id);
                            }
                        }
                        else
                        {

                            Console.WriteLine("Consistency error {0} priceUnit used", priceUnit.Id);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }, cancellationToken).ContinueWith(t => LogEvent(subscription, evnt, cancellationToken));

        }

        private async Task HandleRegion(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
                try
                {
                    var region = JsonSerializer.Deserialize<Region>(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()));
                    if (evnt.Event.EventType != "delete")
                    {

                        if (region != null)
                        {
                            _regions.AddOrUpdate(region.Id, region, (key, oldValue) => region);
                            _products.Where((r) => r.Value.Region == region)
                            .ForEach(p => p.Value.Region = region
                                );

                        }
                    }
                    else
                    {
                        if (_products.Values.All(p => p.RegionGuId != region.Id))
                        {

                            if (_regions.TryRemove(region.Id, out Region c))
                            {
                                Console.WriteLine("{0} remved", region.Id);
                            }
                            else
                            {
                                Console.WriteLine("{0} not removed", region.Id);
                            }
                        }
                        else
                        {

                            Console.WriteLine("Consistency error {0} region used", region.Id);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }, cancellationToken).ContinueWith(t => LogEvent(subscription, evnt, cancellationToken));
        }

        public async Task InitialLoad()
        {
            try
            {

                var currencyEvts = _client.ReadStreamAsync(Direction.Forwards, "Currency.v1", StreamPosition.Start);
                var priceUnitEvts = _client.ReadStreamAsync(Direction.Forwards, "PriceUnit.v1", StreamPosition.Start);
                var regionEvts = _client.ReadStreamAsync(Direction.Forwards, "Region.v1", StreamPosition.Start);
                var productsSubs = _client.ReadStreamAsync(Direction.Forwards, "OfficialProduct.v1", StreamPosition.Start);

                StreamPosition currencyPosition = 0;
                StreamPosition priceUnitPosition = 0;
                StreamPosition regionPosition = 0;
                StreamPosition opPosition = 0;

                if (await currencyEvts.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException("Currency.v1");
                }
                await foreach (var e in currencyEvts)
                {
                    currencyPosition = e.OriginalEventNumber;
                    if (e.Event.EventType == "new")
                    {
                       var currency=  JsonSerializer.Deserialize<Currency>(Encoding.UTF8.GetString(e.Event.Data.ToArray()));
                       if(currency!= null)
                        {
                            _currencies.AddOrUpdate(currency.Id, currency, (key, oldValue) => currency);
                        }
                    }
                }

                if (await priceUnitEvts.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException("PriceUnit.v1");
                }
                await foreach (var e in priceUnitEvts)
                {
                    priceUnitPosition = e.OriginalEventNumber;
                    var pUnit = JsonSerializer.Deserialize<Unit>(Encoding.UTF8.GetString(e.Event.Data.ToArray()));
                    if (pUnit != null)
                    {
                        _priceUnits.AddOrUpdate(pUnit.Id, pUnit, (key, oldValue) => pUnit);
                    }
                }

                if (await regionEvts.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException("Region.v1");
                }
                await foreach (var e in regionEvts)
                {
                    regionPosition = e.OriginalEventNumber;
                    var region = JsonSerializer.Deserialize<Region>(Encoding.UTF8.GetString(e.Event.Data.ToArray()));
                    if (region != null)
                    {
                        _regions.AddOrUpdate(region.Id, region, (key, oldValue) => region);
                    }
                }

                if (await productsSubs.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException("OfficialProduct.v1");
                }
                await foreach (var e in productsSubs)
                {
                    opPosition=e.OriginalEventNumber;
                    var op = JsonSerializer.Deserialize<OfficialProduct>(Encoding.UTF8.GetString(e.Event.Data.ToArray()));
                    if (op != null)
                    {

                        if (_currencies.TryGetValue(op.CurrencyGuId, out Currency c))
                        {
                            op.Currency = c;
                        }
                        if (_priceUnits.TryGetValue(op.UnitGuid, out Unit unit))
                        {
                            op.PriceUnit = unit;
                        }
                        if (op.RegionGuId.HasValue && _regions.TryGetValue(op.RegionGuId.Value, out Region region))
                        {
                            op.Region = region;
                        }
                        _products.AddOrUpdate(op.Id, op, (key, oldValue) => op);
                    }
                }

                _ = await _client.SubscribeToStreamAsync("Currency.v1",FromStream.After(currencyPosition),this.HandleCurrency);
                _ = await _client.SubscribeToStreamAsync("PriceUnit.v1", FromStream.After(priceUnitPosition), this.HandlePriceUnit);
                _ = await _client.SubscribeToStreamAsync("Region.v1", FromStream.After(regionPosition), this.HandleRegion);
                _ = await _client.SubscribeToStreamAsync("OfficialProduct.v1", FromStream.After(opPosition), this.HandleProduct);


                Console.WriteLine("Initial load: {0} OfficialProducts, {1} Currencies, {2} PriceUnits, {3} Regions", _products.Count, _currencies.Count, _priceUnits.Count, _regions.Count);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return;

        }
    }
}
