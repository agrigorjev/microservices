using EventStore.Client;
using System.Text.Json;


namespace MandaraDemoDTO.Contracts
{

    public class MetadataModel
    {

        public string User { get; set; }

        public string Version => "1.0";
        
    }
    public interface IEventData
    {
        Guid Id { get; }
        KnownEvents Event{ get; }
        byte[] Data { get; }
        string User { get; }

        byte[] MetaData => JsonSerializer.SerializeToUtf8Bytes(new MetadataModel() { User=User});

        public EventData toEventData()
        {
            return new EventData(Uuid.FromGuid(Guid.NewGuid()), Event.ToString(), Data, MetaData);
        }

    }

   
}
