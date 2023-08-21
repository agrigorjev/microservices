using EventStore.Client;
using MandaraDemoDTO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficialProductDemoAPI.Extensions;
using System.Text;
using System.Collections.Concurrent;
using MandaraDemoDTO.Extensions;
using OfficialProductDemoAPI.Services.Contracts;
using MandaraDemo.GrpcDefinitions;
using System.Runtime.CompilerServices;

namespace OfficialProductDemoAPI.Services.Cache
{
    public class OfficialProductsCacheService : CacheService<OfficialProduct>
    {
        private const string _streamName = "OfficialProduct.v1";

        private INotifyService<ServiceEventMessage> _notificationService;
        public OfficialProductsCacheService(EventStoreClient client, INotifyService<ServiceEventMessage> notifyService)
        {
            _client = client;
            _cache = new ConcurrentDictionary<Guid, OfficialProduct>();
            _notificationService = notifyService;
           
        }

        public override string StreamName => _streamName;

        public override INotifyService<ServiceEventMessage>? NotificationService { get =>_notificationService; set => _notificationService=value; }



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

        public override INotifyService<ServiceEventMessage>? NotificationService { get => null; set { } }

    
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

        public override INotifyService<ServiceEventMessage>? NotificationService { get => null; set { } }

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

        public override INotifyService<ServiceEventMessage>? NotificationService { get => null; set { } }

    }

}
