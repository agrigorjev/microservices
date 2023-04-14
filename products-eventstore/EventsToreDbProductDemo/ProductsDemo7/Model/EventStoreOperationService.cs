using EventStore.Client;
using JsonDiffPatchDotNet;
using MandaraDemoDTO;
using MandaraDemoDTO.Contracts;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Reactive.Threading.Tasks;


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

        public IObservable<IWriteResult> updateProducts(OfficialProduct changed,OfficialProduct original)
        {
            return exec(ObjectEvent<OfficialProduct>.Update(original, changed, System.Security.Principal.WindowsIdentity.GetCurrent().Name));

        }

        private IObservable<IWriteResult> exec(IEventData de)
        {
            Debug.Print("Run [{0}]: {1}", DateTime.Now, de.Event);
            return _client.AppendToStreamAsync(_streamName, StreamState.StreamExists, new List<EventData>() { de.toEventData() })
               .ToObservable();
        }

    }
}
