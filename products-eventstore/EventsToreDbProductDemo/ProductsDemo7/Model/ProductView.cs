
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Grpc.Net.Client;
using MandaraDemoDTO;
using ProductsDemo.Client;
using ProductsDemo7.Extensions;


namespace ProductsDemo.Model
{
    public class ProductView : IDisposable,INotifyPropertyChanged
    {
        private ProductClientImpl _client;


        private OfficialProductConverter officialProductConverter = new OfficialProductConverter();

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
            Console.WriteLine("dome +"+counter.ToString()+" "+DateTime.Now.ToString());
           
            Loading = true;
             _client.loadAll()
                .Do(result =>
                {
                    _products.Clear();

                    result.Select(p=>officialProductConverter.Convert(p)).Where(p=>p!=null)
                    .ToList()
                    .ForEach(p => {
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
