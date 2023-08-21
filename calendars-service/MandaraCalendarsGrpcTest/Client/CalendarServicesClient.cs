using Grpc.Core;
using Mandara.ProductService.GrpcDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mandara.CalendarsService.GrpcDefinitions.CalendarService;
using static Mandara.ProductService.GrpcDefinitions.ProductAPIService;

namespace Mandara.CalendarsServiceTests.Client
{
    internal class CalendarServicesClient: CalendarServiceClient
    {
    }

    internal class ProductServicesClient : ProductAPIServiceClient
    {
        public ProductServicesClient(ChannelBase channel) : base(channel)
        {
        }
    }
}
