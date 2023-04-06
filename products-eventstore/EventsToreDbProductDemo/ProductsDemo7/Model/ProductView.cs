
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using DevExpress.Mvvm.Native;
using Grpc.Net.Client;
using MandaraDemoDTO;
using ProductsDemo.Client;
using ProductsDemo7.Extensions;
using Region = MandaraDemoDTO.Region;

namespace ProductsDemo.Model
{
    public class ProductView : IDisposable,INotifyPropertyChanged
    {
        private ProductClientImpl _client;
        CancellationToken stopLoad = new CancellationToken();
        private OfficialProductConverter officialProductConverter = new OfficialProductConverter();
        private PriceUnitDataConverter _priceUnitDataConverter = new PriceUnitDataConverter();
        private CurrencyDataConverter _currencyDataConverter = new CurrencyDataConverter();
        private RegionDataConverter _regionDataConverter = new RegionDataConverter();

        private bool _loading = false;

           public string StatusText
        {
            get
            {
                if (_loading) return "Loading...";
                else if (_ex != null) return _ex.Message;
                else return "Loaded " + _products.Count.ToString();
            }
        }

        public bool Loading
        {
            get
            {
                return _loading;
            }
            private set
            {
                _loading = value;
                if (PropertyChanged != null)
                {
                    OnPropertyChanged(nameof(Loading));
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }
        private BindingList<OfficialProduct> _products;

        private ConcurrentDictionary<Guid,Currency> _currencies=new ConcurrentDictionary<Guid, Currency>();
        private ConcurrentDictionary<Guid, Unit> _priceUnits = new ConcurrentDictionary<Guid, Unit>();
        private ConcurrentDictionary<Guid, Region> _regions = new ConcurrentDictionary<Guid, Region>();

        public BindingList<OfficialProduct> Products { get { return _products; } }

        private Exception? _ex =null;

        private int counter=0;

        public Exception? LastError
        {
            get
            {
                return _ex;
            }
            private set
            {
                _ex=value;
                if (PropertyChanged != null)
                {
                    OnPropertyChanged(nameof(LastError));
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }


        public ProductView(string serviceUrl)
        {
            _products = new BindingList<OfficialProduct>();
            _client = new ProductClientImpl(GrpcChannel.ForAddress(serviceUrl));
            DoLoadAll();
        }



        private void DoLoadAll()
        {
            counter++;
            Console.WriteLine("dome +" + counter.ToString() + " " + DateTime.Now.ToString());

            Loading = true;


            _client.loadAllCurrencies()
                .Do(c => {
                    var gotValue = _currencyDataConverter.Convert(c);
                    _currencies.AddOrUpdate(gotValue.Id, gotValue, (key, oldValue) => gotValue);
                })
                .RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    Debug.WriteLine("Retry " + counter++.ToString() + " " + DateTime.Now.ToString());
                    LastError = ex;
                    Loading = false;
                    return true;

                }).Subscribe();

             _client.loadAllPriceUnits()
                .Do(c => {
                    var gotValue = _priceUnitDataConverter.Convert(c);
                    _priceUnits.AddOrUpdate(gotValue.Id, gotValue, (key, oldValue) => gotValue);
                }).RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    Debug.WriteLine("Retry " + counter++.ToString() + " " + DateTime.Now.ToString());
                    LastError = ex;
                    Loading = false;
                    return true;

                }).Subscribe();
            _client.loadAllRegions()
                .Do(c => {
                    var gotValue = _regionDataConverter.Convert(c);
                    _regions.AddOrUpdate(gotValue.Id, gotValue, (key, oldValue) => gotValue);
                }).RetryWithBackoffStrategy(int.MaxValue, n => TimeSpan.FromSeconds(2), ex =>
                {
                    Debug.WriteLine("Retry " + counter++.ToString() + " " + DateTime.Now.ToString());
                    LastError = ex;
                    Loading = false;
                    return true;

                }).Subscribe();

                 _client.loadAll()
                .Do(result =>
                {
                    _products.Clear();

                    result.Select(p=>officialProductConverter.Convert(p)).Where(p=>p!=null)
                    .ToList()
                    .ForEach(p => {
                        if(_currencies.TryGetValue(p.CurrencyGuId, out Currency currency)){
                            p.Currency = currency;
                        }
                        if ( _priceUnits.TryGetValue(p.UnitGuid, out Unit unit))
                        {
                            p.PriceUnit = unit;
                        }
                        if ( p.RegionGuId != null && _regions.TryGetValue(p.CurrencyGuId, out Region region))
                        {
                            p.Region = region;
                        }
                        _products.Add(p);
                        });
                    Loading = false;
                    LastError = null;
                    OnProductsLoaded();
                })
                .RetryWithBackoffStrategy(int.MaxValue, n=>TimeSpan.FromSeconds(2), ex =>
                {
                    Debug.WriteLine("Retry " + counter++.ToString() + " " + DateTime.Now.ToString());
                    LastError = ex;
                    Loading = false;
                    return true;

                })
               .Subscribe();
        }

        public event EventHandler? ProductsLoaded;

        protected void OnProductsLoaded()
        {
            ProductsLoaded?.Invoke(this,new EventArgs());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
           _client.Dispose();
        }
    }
}
