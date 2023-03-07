using System;

namespace Mandara.Business.Authorization
{
    public class EntityIdAttribute : Attribute
    {
        public int Id { get; set; }

        public EntityIdAttribute(int id)
        {
            Id = id;
        }
    }
}