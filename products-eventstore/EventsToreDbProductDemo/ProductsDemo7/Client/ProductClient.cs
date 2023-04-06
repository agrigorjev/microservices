
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




    }
}

