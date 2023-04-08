using EventStore.Client;
using MandaraDemoDTO.Contracts;
using MandaraDemoDTO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MandaraDemoDTO
{

    public class DeleteModel: IReference
    {

        private Guid _Id = Guid.Empty;
        public Guid Id { get =>_Id ; set =>_Id=value; }
    }

    public class DeleteEvent : IEventData
    {

        private Guid _id;
        public string Name { get; set; }
        public DeleteEvent(Guid toRemove)
        { 
            _id = toRemove;
            Name = "N\\A";
        }
        public Guid Id { get
            {
                return _id;
            }
        }

        public KnownEvents Event => KnownEvents.Delete;

        public byte[] Data => JsonSerializer.SerializeToUtf8Bytes<DeleteModel>(new DeleteModel() { Id = _id });

        public string User => Name;
    }


    public class NewOfficialProductEvent : IEventData
    {

        private Guid _id;
        public string Name { get; set; }

        private readonly OfficialProduct _product;

        public NewOfficialProductEvent(OfficialProduct product)
        {
            product.Id = Guid.NewGuid();
            _id = product.Id;
            Name = "N\\A";
            _product = product;
        }
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public KnownEvents Event => KnownEvents.Create;

        public byte[] Data => JsonSerializer.SerializeToUtf8Bytes<OfficialProduct>(_product);

        public string User => Name;
    }


    public class UpdateOfficialProductEvent : IEventData
    {

        private Guid _id;
        public string Name { get; set; }

        private readonly OfficialProduct _product;

        public UpdateOfficialProductEvent(OfficialProduct product)
        {
            _id = product.Id;
            Name = "N\\A";
            _product = product;
        }
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public KnownEvents Event => KnownEvents.Update;

        public byte[] Data => JsonSerializer.SerializeToUtf8Bytes<OfficialProduct>(_product);

        public string User => Name;
    }


    public class ObjectEvent<T> : IEventData where T : IReference
    {
        private Guid _id;

        public Guid Id => _id;

        private KnownEvents _event;

        public KnownEvents Event => _event;

        private T _subject;

        public T Subject { get => _subject; }

        public byte[] Data => _event==KnownEvents.Delete? JsonSerializer.SerializeToUtf8Bytes<DeleteModel>(new DeleteModel() { Id=_id}) : JsonSerializer.SerializeToUtf8Bytes<T>(_subject);

        private string _metadataUser;
        public string User => _metadataUser;

        private ObjectEvent(){
        }
        public static ObjectEvent<T> Create(T subject,string userName)
        {
            subject.Id=Guid.NewGuid();
            return new ObjectEvent<T>() { _id = subject.Id, _subject = subject, _metadataUser = userName ?? string.Empty, _event = KnownEvents.Create };
        }

        public static ObjectEvent<T> Update(T subject, string userName)
        {
            return new ObjectEvent<T>() { _id = subject.Id, _subject = subject, _metadataUser = userName ?? string.Empty, _event = KnownEvents.Update };
        }

        public static ObjectEvent<T> Delete(T subject, string userName)
        {
            return new ObjectEvent<T> () { _id = subject.Id, _event = KnownEvents.Delete, _metadataUser = userName ?? string.Empty };
        }

        public static ObjectEvent<T> fromEventData(EventRecord record)
        {
            try
            {
                var jsonContent = Encoding.UTF8.GetString(record.Data.ToArray());
                MetadataModel? metadataModel = null;
                if (record.Metadata.Length > 0)
                {
                    try
                    {
                       metadataModel = JsonSerializer.Deserialize<MetadataModel>(record.Metadata.ToArray());
                    }
                    catch { }
                }
                Guid idFrom=JsonNode.Parse(jsonContent)["Id"].GetValue<Guid>();

                if (idFrom!=Guid.Empty)
                {
                    var eType=record.EventType.toKnownEvent();
                    if (eType == KnownEvents.Delete)
                    {
                        return new ObjectEvent<T>() { _id = idFrom, _event = KnownEvents.Delete, _metadataUser = metadataModel?.User };
                    }
                    else
                    {
                        return new ObjectEvent<T>() { _id = idFrom, _subject= JsonSerializer.Deserialize<T>(jsonContent), _event =eType, _metadataUser = metadataModel?.User };
                    }
                    
                }
                else
                {
                    throw new WrongEventFormatException();
                }
            }
            catch 
            {
                throw;
            }
        }

       


    }

}
