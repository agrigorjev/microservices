
using Google.Protobuf.Collections;
using Grpc.Core;
using MandaraDemo.GrpcDefinitions;
using System.Reactive.Linq;
using static MandaraDemo.GrpcDefinitions.ProductAPIService;

namespace ProductsDemo.Client
{
    public class ProductClientImpl: ProductAPIServiceClient,IDisposable
    {
        ChannelBase _channel;


        public ProductClientImpl(ChannelBase channel):base(channel)
        {
            _channel = channel;
        }

        public void Dispose()
        {
           _channel.ShutdownAsync().Wait();
        }

        public IObservable<RepeatedField<ProductGrpc>> loadAll()
        {
             return Observable.FromAsync(() => GetAllProductsAsync(new GetAllRequestMessage()).ResponseAsync).Select(x => x.Products);
            
        }

        public IObservable<CurrencyGrpc> loadAllCurrencies()
        {
            return Observable.FromAsync(() => GetCurrencyReferenceAsync(new GetAllRequestMessage()).ResponseAsync).Select(x => x.Reference)
                 .SelectMany(lst =>lst.Select(grpc => grpc)) ;

        }

        public IObservable<PriceUnitGrpc> loadAllPriceUnits()
        {
            return Observable.FromAsync(() => GetPriceUnitReferenceAsync(new GetAllRequestMessage()).ResponseAsync).Select(x => x.Reference)
                 .SelectMany(lst => lst.Select(grpc => grpc));

        }


        public IObservable<RegiontGrpc> loadAllRegions()
        {
            return Observable.FromAsync(() => GetRegionReferenceAsync(new GetAllRequestMessage()).ResponseAsync).Select(x => x.Reference)
                 .SelectMany(lst => lst.Select(grpc => grpc));

        }


    }
}

