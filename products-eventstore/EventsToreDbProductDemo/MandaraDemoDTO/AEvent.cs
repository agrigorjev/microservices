using MandaraDemoDTO.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MandaraDemoDTO
{

    class DeleteModel: IReferece
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
}
