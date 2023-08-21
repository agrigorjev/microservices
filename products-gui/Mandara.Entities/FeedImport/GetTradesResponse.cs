using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mandara.Entities.FeedImport
{
    [DataContract]
    [KnownType("GetKnownTypes")]
    public class GetTradesResponse
    {
        
        public GetTradesResponse()
        {
            FeedTrades = new List<FeedTradeDTO>();
            Messages = new List<FeedImportMessage>();
        }

        static IEnumerable<Type> GetKnownTypes()
        {
            return new[] { typeof(GetTradesResponse) };
        }

        public static GetTradesResponse Create(FeedImportMessage message, List<FeedTradeDTO> feedTrades)
        {
            return Create(new FeedImportMessage[] { message }, feedTrades);
        }

        public static GetTradesResponse Create(IEnumerable<FeedImportMessage> messages, List<FeedTradeDTO> feedTrades)
        {
            var result = Create(feedTrades);
            foreach (var each in messages)
            {
                result.Messages.Add(each);
            }
            return result;
        }

        public static GetTradesResponse Create(List<FeedTradeDTO> feedTrades)
        {
            return new GetTradesResponse() { FeedTrades = feedTrades };
        }

        [DataMember]
        public IList<FeedTradeDTO> FeedTrades { get; set; }

        [DataMember]
        public IList<FeedImportMessage> Messages { get; set; }
      }
}
