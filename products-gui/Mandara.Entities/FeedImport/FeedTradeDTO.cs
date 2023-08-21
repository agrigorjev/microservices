using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mandara.Data;

namespace Mandara.Entities.FeedImport
{
    public enum FeedSourceType
    {
        ICE = 0,
        ClearPort,
        Anb,
        Database
    }

    [DataContract(IsReference = true)]
    public class FeedTradeDTO
    {
        [DataMember]
        public DateTime? TradeDate { get; set; }

        [DataMember]
        public string ExecID { get; set; }

        [DataMember]
        public string LegRefID { get; set; }

        [DataMember]
        public string ClientOrderID { get; set; }

        [DataMember]
        public string OrderID { get; set; }

        [DataMember]
        public TradeCaptureSide Side { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Product { get; set; }

        [DataMember]
        public decimal? Quantity { get; set; }

        public decimal? QuantityForComparison 
        {
            get 
            {
                if (!Quantity.HasValue)
                    return null;

                if (Quantity % 1000 == 0)
                    return Math.Abs(Quantity.Value / 1000);
                return Math.Abs(Quantity.Value);
            }
        }
        
        [DataMember]
        public decimal? Price { get; set; }

        [DataMember]
        public string Strip { get; set; }

        [DataMember]
        public FeedSourceType FeedSource { get; set; }

        private List<FeedTradeFieldDTO> _additionalFields = new List<FeedTradeFieldDTO>();

        [DataMember]
        public List<FeedTradeFieldDTO> AdditionalFields
        {
            get { return _additionalFields; }
            set { _additionalFields = value; }
        }

        private List<FeedTradeErrorMessageDTO> _errors = new List<FeedTradeErrorMessageDTO>();

        [DataMember]
        public List<FeedTradeErrorMessageDTO> Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }
    }
}