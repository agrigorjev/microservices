
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Extensions;
using EventStore.Client;
using Grpc.Core;
using Grpc.Net.Client;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using NLog;
using ProductsDemo.Client;
using ProductsDemo7.Extensions;
using ProductsDemo7.Model;
using Region = MandaraDemoDTO.Region;

namespace ProductsDemo.Model
{
    public class ProductView : IDisposable
    {

        private ProductClientImpl _client;
        private OfficialProductConverter officialProductConverter = new OfficialProductConverter();
        private PriceUnitDataConverter _priceUnitDataConverter = new PriceUnitDataConverter();
        private CurrencyDataConverter _currencyDataConverter = new CurrencyDataConverter();
        private RegionDataConverter _regionDataConverter = new RegionDataConverter();

        private Logger _logger = LogManager.GetCurrentClassLogger();
        private ConcurrentDictionary<Guid,Currency> _currencies=new ConcurrentDictionary<Guid, Currency>();
        private ConcurrentDictionary<Guid, Unit> _priceUnits = new ConcurrentDictionary<Guid, Unit>();
        private ConcurrentDictionary<Guid, Region> _regions = new ConcurrentDictionary<Guid, Region>();

        private BindingList<Currency> _currencySrc=new BindingList<Currency>();
        public BindingList<Currency> CurrencySrc => _currencySrc;

        private BindingList<Unit> _priceUnitSrc = new BindingList<Unit>();
        public BindingList<Unit> PriceUnitSrc => _priceUnitSrc;

        private BindingList<Region> _regionSrc = new BindingList<Region>();
        public BindingList<Region> RegionSrc => _regionSrc;

        private BindingList<OfficialProduct> _productsSrc = new BindingList<OfficialProduct>();
        public BindingList<OfficialProduct> ProductsSrc => _productsSrc;

        private TaskScheduler _uiScheduler;

        private void AddOrUpdate(OfficialProduct product,bool replace=false)
        {
            var idx=_productsSrc.IndexOf(p => p.Id == product.Id);
            if(idx>=0)
            {
                if (replace)
                {
                    _productsSrc.RemoveAt(idx);
                    _productsSrc.Add(product);
                }
            }
            else
            {
                _productsSrc.Add(product);
            }
           
        }

        private readonly EventStoreOperationService _storeService;

        private Subject<string> _stateMessages=new Subject<string>();

        public ProductView(string serviceUrl, TaskScheduler uiScheduler)
        {
            _client = new ProductClientImpl(GrpcChannel.ForAddress(serviceUrl));
            _uiScheduler=uiScheduler;
            _storeService = new EventStoreOperationService();
            
        }

        public IDisposable onStatusChanged(Action<String> action)
        {
            return _stateMessages.Subscribe(s=>Extensions.onTaskScheduler(()=>action(s), ex => _logger.Error(ex, "Update status text"), _uiScheduler));
        }

        public void Init(Action<BindingList<OfficialProduct>> bindAction)
        {
            LoadReferences();
            GetAll()
                 .ToList()
                 .Subscribe(lst =>
                 {
                     _productsSrc = new BindingList<OfficialProduct>(lst);
                     bindAction.Invoke(ProductsSrc);
                 },
                  onCompleted: () =>
                  {
                      _stateMessages.OnNext(string.Format("Products loaded. Total {0}", _productsSrc.Count));
                      _client.EventStream.ReadAllAsync().ToObservable<ServiceEventMessage>()
                         .Subscribe(HandleServiceEventMessage);
                  });
            

        }

        private void HandleServiceEventMessage(ServiceEventMessage message)
        {
            switch (message.EventType)
            {
                case "OfficialProduct":
                    {
                        message.EventPayload.ToList().ForEach(payloadItem =>
                        {
                            if (payloadItem.StartsWith("Warmup"))
                            {
                                _logger.Info("Got OfficialProduct {0}", message.EventType);
                                GetAll().Subscribe(p =>
                                     Extensions.onTaskScheduler(() => AddOrUpdate(p), ex => _logger.Error(ex, "ProductSource update error"), _uiScheduler)
                                );
                            }
                            else if(Guid.TryParse(payloadItem,out Guid toReload))
                            {
                                _logger.Info("Got OfficialProduct {0} for {1}", message.EventType,payloadItem);
                                loadSingleProduct(toReload);
                            }
                        });
                    }
                    break;
                default:
                    _logger.Info("Got event: {0}",message.EventType);
                    break;
            }
        }

        private OfficialProduct enrichOfficialProduct(OfficialProduct p)
        {
            if (p != null)
            {
                if (_currencies.TryGetValue(p.CurrencyGuId, out Currency? currency))
                {
                    p.Currency = currency;
                }
                if (_priceUnits.TryGetValue(p.UnitGuid, out Unit? unit))
                {
                    p.PriceUnit = unit;
                }
                if (p.RegionGuId != null && _regions.TryGetValue(p.CurrencyGuId, out Region? region))
                {
                    p.Region = region;
                }
            }
            return p;
        }

        private void loadSingleProduct(Guid toLoad)
        {
            Observable.FromAsync(_ => _client.SingleProduct(toLoad).ResponseAsync)
                 .Where(p=>p.Product!=null)
                 .Select(p => enrichOfficialProduct(officialProductConverter.Convert(p.Product)))
                 .Subscribe(p =>
                 {
                     Extensions.onTaskScheduler(()=>AddOrUpdate(p,true),ex => _logger.Error(ex, "ProductSource update error"), _uiScheduler);
                 },
                 _=>{
                     _stateMessages.OnNext(string.Format("Update {0} done.", toLoad));
                 });
        }

        private IObservable<OfficialProduct> GetAll()
        {
           var retryCounter = 1;
           return _client.loadAllProducts()
                .Where(p => p != null)
                .Select(p => enrichOfficialProduct(p: officialProductConverter.Convert(p)))
                .Where(p => p != null)
                .RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    _logger.Warn(ex, "Curency init {0}", retryCounter++);
                    return true;
                });
        }
        private void LoadReferences()
        {
            int retryCounter =1;

            _client.loadAllCurrencies()
                .Select(_currencyDataConverter.Convert)
                .Where(c => c != null)
                .RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    _logger.Warn(ex, "Curency init {0}", retryCounter++);
                    _stateMessages.OnNext(string.Format("Curency init retry {0}", retryCounter++));
                    return true;

                }, scheduler:Scheduler.Immediate)
                .Subscribe(c => {
                    _currencies.AddOrUpdate(c.Id, c, (key, oldValue) => c);
                },
                 onCompleted: () => {
                     Extensions.onTaskScheduler(() => _currencySrc.fromDictionary(_currencies), ex => _logger.Error(ex, "Bind list error"), _uiScheduler);
                     _logger.Info("Curency init {0} done.Total {1}", retryCounter, _currencies.Count);
                    _stateMessages.OnNext(string.Format("Curency init {0} done.Total {1}", retryCounter, _currencies.Count));
                });
            _stateMessages.OnNext("Start loading price units");
            retryCounter = 1;
            _client.loadAllPriceUnits()
                .Select(_priceUnitDataConverter.Convert)
                .Where(c => c != null)
                .RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    _logger.Warn(ex, "PriceUnit init {0}", retryCounter++);
                    _stateMessages.OnNext(string.Format("PriceUnit init retry {0}", retryCounter++));
                    return true;

                }, scheduler: Scheduler.Immediate)
                .SubscribeOn(Scheduler.Immediate)
                .Subscribe(gotValue => {
                    _priceUnits.AddOrUpdate(gotValue.Id, gotValue, (key, oldValue) => gotValue);
                }, 
                onCompleted: () => {
                    Extensions.onTaskScheduler(() => _priceUnitSrc.fromDictionary(_priceUnits), ex => _logger.Error(ex, "Bind list error"), _uiScheduler);
                    _logger.Info("PriceUnit init {0} done.Total {1}", retryCounter, _priceUnits.Count); 
                    _stateMessages.OnNext(string.Format("PriceUnit init {0} done.Total {1}", retryCounter, _priceUnits.Count));
                });

            
            retryCounter = 1;
             _client.loadAllRegions()
                .Select(_regionDataConverter.Convert)
                .Where(c => c != null)
                .RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    _logger.Warn(ex, "Region init {0}", retryCounter++);
                    _stateMessages.OnNext(string.Format("Region init retry {0}", retryCounter++));
                    return true;

                }, scheduler: Scheduler.Immediate)
                .SubscribeOn(Scheduler.Immediate)
                .Subscribe(gotValue => {
                    _regions.AddOrUpdate(gotValue.Id, gotValue, (key, oldValue) => gotValue);
                },
                onCompleted: () => {
                    Extensions.onTaskScheduler(() => _regionSrc.fromDictionary(_regions), ex => _logger.Error(ex, "Bind list error"), _uiScheduler);
                    _logger.Info( "Region init {0} done.Total {1}", retryCounter, _regions.Count);
                    _stateMessages.OnNext(string.Format("Region init {0} done.Total {1}", retryCounter, _regions.Count));
                });
        }
        
        private void RemoveFromList(Guid id)
        {
            var idx =_productsSrc.IndexOf(p => p.Id == id);
            if (idx >= 0)
            {
                Extensions.onTaskScheduler(()=>_productsSrc.RemoveAt(idx), ex => _logger.Error(ex),_uiScheduler);
            }
        }

        public void DeleteProduct(OfficialProduct p)
        {
            _storeService
                .deleteProducts(p)
                 .SubscribeOn(Scheduler.Default)
                .Subscribe(wr =>
                {
                    _logger.Info("Sent {0} with position {1}", p.Id, wr.LogPosition);
                    loadSingleProduct(p.Id);
                });
        }
        public void AddProduct(OfficialProduct p)
        {
            _storeService
                .createProducts(p)
                .SubscribeOn(Scheduler.Default)
                .Subscribe(wr =>
                {
                    RemoveFromList(p.Id);
                    _logger.Info("Sent {0} with position {1}", p.Id, wr.LogPosition);
                    loadSingleProduct(p.Id);
                });
        }

        private OfficialProduct? _inEdit = null;
        public void BeginUpdate(int opIndex)
        {
            try
            {
                var product= _productsSrc[opIndex];
                if(product!=null && !product.isNew)
                {
                    _inEdit = product.CloneJson();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void CancelUpdate()
        {
            _inEdit = null;
        }

        public void ExecUpdate(OfficialProduct changed)
        {
            if (_inEdit != null && changed.Id == _inEdit.Id)
            {
                _storeService
                  .updateProducts(changed,_inEdit)
                   .SubscribeOn(Scheduler.Default)
                   .Subscribe(wr =>
                   {
                       RemoveFromList(changed.Id);
                       _logger.Info("Sent update {0} with position {1}", changed.Id, wr.LogPosition);
                       loadSingleProduct(changed.Id);
                   });
            }
            else
            {
                _logger.Warn("Update inconsistent");
            }
        }

        public void Dispose()
        {
           _client.Dispose();
        }
    }

    
}
