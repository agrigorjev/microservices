using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.TradeAdd;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.TradeAdd
{
    public class TradeAddCreateRequestMessage : MessageBase
    {
        public TradeAddDetails TradeAddDetails { get; set; }
        public bool IsMasterToolMode { get; set; }
    }

    public class TradeAddCreateRequestMessageDto : MessageBase
    {
        public TradeAddDetailsDto TradeAddDetails { get; set; }
        public bool IsMasterToolMode { get; set; }
    }

    public class TradeAddCreateResponseMessage : MessageWithErrorBase
    {
        public List<int> InsertedTradeIds { get; set; }

        public TradeAddCreateResponseMessage()
        {
            InsertedTradeIds = new List<int>();
        }

        public override void OnErrorSet()
        {
            InsertedTradeIds.Clear();
        }
    }

    public class TradeAddCreateResponseMessageDto : MessageWithErrorBase
    {
        public List<int> InsertedTradeIds { get; set; }

        public TradeAddCreateResponseMessageDto()
        {
            InsertedTradeIds = new List<int>();
        }

        public override void OnErrorSet()
        {
            InsertedTradeIds.Clear();
        }
    }
}