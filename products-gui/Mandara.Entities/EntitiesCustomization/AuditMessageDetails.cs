using System.Collections.Generic;

namespace Mandara.Entities
{
    public class AuditMessageDetails
    {
        public string Property { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public List<AuditMessageDetails> Children { get; set; } = new List<AuditMessageDetails>();

        public int Id { get; set; } = -1;

        public int ParentId { get; set; }

        public override string ToString()
        {
            return $"Property: {Property}, Old Value: {OldValue}, New Value: {NewValue}.";
        }
    }
}