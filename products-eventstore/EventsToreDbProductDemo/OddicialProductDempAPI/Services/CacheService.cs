using EventStore.Client;
using MandaraDemoDTO;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace OfficialProductDemoAPI.Services

{
    public class CacheService : IDataService<OfficialProduct> 
    { 


        private ConcurrentDictionary<Guid,OfficialProduct> _products=new ConcurrentDictionary<Guid, OfficialProduct>();
        private ConcurrentDictionary<Guid, Unit> _priceUnits = new ConcurrentDictionary<Guid, Unit>();
        private ConcurrentDictionary<Guid, Currency> _currencies = new ConcurrentDictionary<Guid, Currency>();
        private ConcurrentDictionary<Guid, Region> _regions = new ConcurrentDictionary<Guid, Region>();

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

        public async Task InitialLoad()
        {
            try
            {

                var currencyEvts = _client.ReadStreamAsync(Direction.Forwards, "Currency.v1", StreamPosition.Start);
                var priceUnitEvts = _client.ReadStreamAsync(Direction.Forwards, "PriceUnit.v1", StreamPosition.Start);
                var regionEvts = _client.ReadStreamAsync(Direction.Forwards, "Region.v1", StreamPosition.Start);
                var productsSubs = _client.ReadStreamAsync(Direction.Forwards, "OfficialProduct.v1", StreamPosition.Start);

                if (await currencyEvts.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException("Currency.v1");
                }
                await foreach (var e in currencyEvts)
                {
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
