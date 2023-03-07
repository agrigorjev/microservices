using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages
{
    public class LiveData
    {
        public int TradeId { get; set; }

        public Money? LivePrice { get; set; }

        public Money? PnL { get; set; }

        public Money? LivePnLBase { get; set; }

        public Money? OvernightPrice { get; set; }

        public Money? LiveCosts { get; set; }

        public string Currency {
            get
            {
                return LivePrice != null ? LivePrice.Value.Currency :
                    (PnL != null ? PnL.Value.Currency :
                    (LivePnLBase != null ? LivePnLBase.Value.Currency :
                    (OvernightPrice != null ? OvernightPrice.Value.Currency : 
                    (LiveCosts != null ? LiveCosts.Value.Currency : null))));
            }
        }
    }

    public class LiveDataMessage : MessageBase
    {
        public LiveDataMessage()
        {
            LiveDataCollection = new List<LiveData>();
        }

        public LiveDataMessage(IList<LiveData> liveData)
            : this()
        {
            if (liveData != null)
                LiveDataCollection.AddRange(liveData);
        }

        public List<LiveData> LiveDataCollection { get; set; }
    }
}
