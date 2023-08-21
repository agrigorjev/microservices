using System.Runtime.Serialization;

namespace Mandara.Entities.FeedImport
{
    [DataContract(IsReference = true)]
    public class FeedTradeFieldDTO
    {
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public string Value { get; set; }

        public FeedTradeFieldDTO(string field, string value)
        {
            Field = field;
            Value = value;
        }


        public FeedTradeFieldDTO(string field, object value):this(field, (value == null)? string.Empty: value.ToString())
        {}
    }
}