using EventStore.Client;
using JsonDiffPatchDotNet;
using MandaraDemoDTO.Contracts;
using MandaraDemoDTO.Extensions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace MandaraDemoDTO
{

    public class DeleteModel : IReference
    {

        private Guid _Id = Guid.Empty;
        public Guid Id { get => _Id; set => _Id = value; }
    }

    public class UpdateModel<T> where T : IReference
    {
        public string PatchDocument { get; set; }

        public Guid Id { get; set; }

        public UpdateModel()
        {
            Id = Guid.Empty;
            PatchDocument = "";
        }
        public UpdateModel(T original, T updated)
        {
            Id = original.Id;
            PatchDocument = buildPatch(original, updated);
        }

        protected string buildPatch(T original, T changed)
        {

            var jsp = new JsonDiffPatch();
            return jsp.Diff(JToken.FromObject(original), JToken.FromObject(changed)).ToString();
          
        }

        public T withUpdate(T original)
        {
            if (!string.IsNullOrEmpty (PatchDocument))
            {
                try
                {
                    var jsp = new JsonDiffPatch();
                    return jsp.Patch(JToken.FromObject(original), JToken.Parse(PatchDocument)).ToObject<T>();
                }
                catch(Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
            return original;
        }
    }

    public class ObjectEvent<T> : IEventData where T : IReference
    {
        private Guid _id;

        public Guid Id => _id;

        private KnownEvents _event;

        public KnownEvents Event => _event;

        private T _subject;

        public T Subject { get => _subject; }

        public byte[] Data
        {
            get
            {
                switch (_event)
                {
                    case KnownEvents.Delete:
                        return JsonSerializer.SerializeToUtf8Bytes<DeleteModel>(new DeleteModel() { Id = _id });
                    case KnownEvents.Update:
                        return JsonSerializer.SerializeToUtf8Bytes<UpdateModel<T>>(_updateModel);
                    case KnownEvents.Create:
                        return JsonSerializer.SerializeToUtf8Bytes<T>(_subject);
                    default:
                        return UTF8Encoding.UTF8.GetBytes("{}");
                }
            }
        }

        private string _metadataUser;
        public string User => _metadataUser;

        private UpdateModel<T> _updateModel;

        private ObjectEvent(){
        }
        public static ObjectEvent<T> Create(T subject,string userName)
        {
            subject.Id=Guid.NewGuid();
            return new ObjectEvent<T>() { _id = subject.Id, _subject = subject, _metadataUser = userName ?? string.Empty, _event = KnownEvents.Create };
        }

        public static ObjectEvent<T> Update(T original, T changed, string userName)
        {
            return new ObjectEvent<T>() { _id = original.Id,_updateModel=new UpdateModel<T>(original,changed), _metadataUser = userName ?? string.Empty, _event = KnownEvents.Update };
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
                    switch (eType)
                    {
                        case KnownEvents.Delete:
                            return new ObjectEvent<T>() { _id = idFrom, _event = KnownEvents.Delete, _metadataUser = metadataModel?.User };
                        case KnownEvents.Update:
                            return new ObjectEvent<T>() { _id = idFrom, _updateModel = JsonSerializer.Deserialize<UpdateModel<T>>(jsonContent), _event = KnownEvents.Update, _metadataUser = metadataModel?.User };
                        case KnownEvents.Create:
                            return new ObjectEvent<T>() { _id = idFrom, _subject = JsonSerializer.Deserialize<T>(jsonContent), _event = KnownEvents.Create, _metadataUser = metadataModel?.User };
                        default:
                            throw new WrongEventFormatException();


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

        public T withUpdate(T toUpdate)
        {
            if(_event!=KnownEvents.Update || _updateModel == null) 
                return toUpdate;
            return _updateModel.withUpdate(toUpdate);
        }


    }

}
