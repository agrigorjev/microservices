
using Google.Protobuf.Collections;
using Grpc.Core;
using System.Reactive.Linq;
using ProductsDemo.GrpcDefinitions;
using static ProductsDemo.GrpcDefinitions.ProductAPIService;
using System;

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

