using EventStore.Client;
using MandaraDemoDTO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficialProductDemoAPI.Extensions;
using System.Text;
using System.Collections.Concurrent;
using MandaraDemoDTO.Extensions;

namespace OfficialProductDemoAPI.Services.Cache
{
    public class OfficialProductsCacheService : CacheService<OfficialProduct>
    {
        private const string _streamName = "OfficialProduct.v1";
        public OfficialProductsCacheService(EventStoreClient client)
        {
            _client = client;
            _cache = new ConcurrentDictionary<Guid, OfficialProduct>();
           
        }

        public override string StreamName => _streamName;

        //protected override OfficialProduct? FromEventData(EventRecord record)
        //{
        //    var jsonContent = Encoding.UTF8.GetString(record.Data.ToArray());
        //    var jObject = JObject.Parse(jsonContent);
        //    Guid id = Guid.Empty;
        //    Guid.TryParse(jObject.SelectToken("Id").ToString() ?? "", out id);
        //    var serializer = Newtonsoft.Json.JsonSerializer.Create();
        //    switch (record.EventType.toKnownEvent())
        //    {
        //        case KnownEvents.Create:
        //            if (id != Guid.Empty && _cache.ContainsKey(id))
        //            {
        //                Console.WriteLine("Object already created with id {0}", id);
        //                return null;
        //            }
        //            return JsonConvert.DeserializeObject<OfficialProduct>(jsonContent);
        //        case KnownEvents.Update:
        //            return JsonConvert.DeserializeObject<OfficialProduct>(jsonContent);
        //        default:
        //            Console.WriteLine("Unknown event type {0}", record.EventType);
        //            return null;
        //    }



        //}


    }

    public class CurrencyCacheService : CacheService<Currency>
    {
        private const string _streamName = "Currency.v1";
        public CurrencyCacheService(EventStoreClient client)
        {
            _client = client;
            _cache = new ConcurrentDictionary<Guid, Currency>();
        }

        public override string StreamName => _streamName;

        //protected override Currency? FromEventData(EventRecord record)
        //{
        //    var jsonContent = Encoding.UTF8.GetString(record.Data.ToArray());
        //    var jObject = JObject.Parse(jsonContent);
        //    Guid id = Guid.Empty;
        //    Guid.TryParse(jObject.SelectToken("Id").ToString() ?? "", out id);
        //    var serializer = Newtonsoft.Json.JsonSerializer.Create();
        //    switch (record.EventType.toKnownEvent())
        //    {
        //        case KnownEvents.Create:
        //            if (id != Guid.Empty && _cache.ContainsKey(id))
        //            {
        //                Console.WriteLine("Object already created with id {0}", id);
        //                return null;
        //            }
        //            return JsonConvert.DeserializeObject<Currency>(jsonContent);
        //        case KnownEvents.Update:
        //            return JsonConvert.DeserializeObject<Currency>(jsonContent);
        //        default:
        //            Console.WriteLine("Unknown event type {0}", record.EventType);
        //            return null;
        //    }



        //}


    }

    public class UnitCacheService : CacheService<Unit>
    {
        private const string _streamName = "PriceUnit.v1";
        public UnitCacheService(EventStoreClient client)
        {
            _client = client;
            _cache = new ConcurrentDictionary<Guid, Unit>();
        }

        public override string StreamName => _streamName;

        //protected override Unit? FromEventData(EventRecord record)
        //{
        //    var jsonContent = Encoding.UTF8.GetString(record.Data.ToArray());
        //    var jObject = JObject.Parse(jsonContent);
        //    Guid id = Guid.Empty;
        //    Guid.TryParse(jObject.SelectToken("Id").ToString() ?? "", out id);
        //    var serializer = Newtonsoft.Json.JsonSerializer.Create();
        //    switch (record.EventType.toKnownEvent())
        //    {
        //        case KnownEvents.Create:
        //            if (id != Guid.Empty && _cache.ContainsKey(id))
        //            {
        //                Console.WriteLine("Object already created with id {0}", id);
        //                return null;
        //            }
        //            return JsonConvert.DeserializeObject<Unit>(jsonContent);
        //        case KnownEvents.Update:
        //            return JsonConvert.DeserializeObject<Unit>(jsonContent);
        //        default:
        //            Console.WriteLine("Unknown event type {0}", record.EventType);
        //            return null;
        //    }



        //}


    }

    public class RegionCacheService : CacheService<Region>
    {
        private const string _streamName = "Region.v1";
        public RegionCacheService(EventStoreClient client)
        {
            _client = client;
            _cache = new ConcurrentDictionary<Guid, Region>();
            
        }

        public override string StreamName => _streamName;

        //protected override Region? FromEventData(EventRecord record)
        //{
        //    var jsonContent = Encoding.UTF8.GetString(record.Data.ToArray());
        //    var jObject = JObject.Parse(jsonContent);
        //    Guid id = Guid.Empty;
        //    Guid.TryParse(jObject.SelectToken("Id").ToString() ?? "", out id);
        //    var serializer = Newtonsoft.Json.JsonSerializer.Create();
        //    switch (record.EventType.toKnownEvent())
        //    {
        //        case KnownEvents.Create:
        //            if (id != Guid.Empty && _cache.ContainsKey(id))
        //            {
        //                Console.WriteLine("Object already created with id {0}", id);
        //                return null;
        //            }
        //            return JsonConvert.DeserializeObject<Region>(jsonContent);
        //        case KnownEvents.Update:
        //            return JsonConvert.DeserializeObject<Region>(jsonContent);
        //        default:
        //            Console.WriteLine("Unknown event type {0}", record.EventType);
        //            return null;
        //    }



        //}


    }

}
