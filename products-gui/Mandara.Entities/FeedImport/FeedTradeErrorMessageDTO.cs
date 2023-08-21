using System.Runtime.Serialization;

namespace Mandara.Entities.FeedImport
{
    [DataContract(IsReference = true)]
    public class FeedTradeErrorMessageDTO
    {
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public string Error { get; set; }

        public FeedTradeErrorMessageDTO(string field, string error)
        {
            Field = field;
            Error = error;
        }
    }
}