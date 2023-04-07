using DevExpress.Diagram.Core.Shapes.Native;
using EventStore.Client;
using MandaraDemoDTO;
using MandaraDemoDTO.Contracts;
using ProductsDemo7.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace ProductsDemo7.Model
{
    public class EventStoreOperationService
    {
        private readonly EventStoreClient _client;
        private readonly string _streamName;

        public EventStoreOperationService()
        {
            _streamName = ConfigurationManager.AppSettings["OfficialProductsStreamName"] ?? "OfficialProduct.v1";
            _client = new EventStoreClient(EventStoreClientSettings.Create(ConfigurationManager.AppSettings["eventStoreConnectionString"] ?? "esdb://localhost:2113?tls=false"));
        }


        public  IObservable<IWriteResult> deleteProducts(OfficialProduct product)
        {
            DeleteEvent de = new(product.Id) { Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name };
            return exec(de);

        }

        public IObservable<IWriteResult> createProducts(OfficialProduct product)
        {
            NewOfficialProductEvent de = new(product) { Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name };
            return exec(de);

        }

        public IObservable<IWriteResult> updateProducts(OfficialProduct product)
        {
            UpdateOfficialProductEvent de = new(product) { Name = System.Security.Principal.WindowsIdentity.GetCurrent().Name };
            return exec(de);

        }


        private IObservable<IWriteResult> exec(IEventData de)
        {
            Debug.Print("Run [{0}]: {1}", DateTime.Now, de.Event);
            return _client.AppendToStreamAsync(_streamName, StreamState.StreamExists, new List<EventData>() { de.toEventData() })
               .ToObservable();
        }

    }
}
