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
            return exec(ObjectEvent<OfficialProduct>.Delete(product, System.Security.Principal.WindowsIdentity.GetCurrent().Name));
        }

        public IObservable<IWriteResult> createProducts(OfficialProduct product)
        {
            return exec(ObjectEvent<OfficialProduct>.Create(product, System.Security.Principal.WindowsIdentity.GetCurrent().Name));

        }

        public IObservable<IWriteResult> updateProducts(OfficialProduct product)
        {
            return exec(ObjectEvent<OfficialProduct>.Update(product, System.Security.Principal.WindowsIdentity.GetCurrent().Name));

        }

        private IObservable<IWriteResult> exec(IEventData de)
        {
            Debug.Print("Run [{0}]: {1}", DateTime.Now, de.Event);
            return _client.AppendToStreamAsync(_streamName, StreamState.StreamExists, new List<EventData>() { de.toEventData() })
               .ToObservable();
        }

    }
}
