using EventStore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
