
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

        CancellationTokenSource _cancellationSrc= new CancellationTokenSource();

        private readonly AsyncServerStreamingCall<ServiceEventMessage> _asyncServerStreamingCall;

        public IAsyncStreamReader<ServiceEventMessage> EventStream => _asyncServerStreamingCall.ResponseStream;
        public ProductClientImpl(ChannelBase channel):base(channel)
        {
            _channel = channel;
            _asyncServerStreamingCall = base.StreamNotify(new GetAllRequestMessage());

        }


        public void Dispose()
        {
           
           _channel.ShutdownAsync().Wait();
            _asyncServerStreamingCall.Dispose();
        }

        public IObservable<ProductGrpc> loadAllProducts()
        {
             return Observable.FromAsync(() => GetAllProductsAsync(new GetAllRequestMessage()).ResponseAsync)
                .SelectMany(x => x.Products);
            
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


        public AsyncUnaryCall<ProductGrpcResponse> SingleProduct(Guid id)
        {
           return base.GetProductAsync(new GetByIdRequestMessage() { Id = id.ToString() });
        }


    }
}

