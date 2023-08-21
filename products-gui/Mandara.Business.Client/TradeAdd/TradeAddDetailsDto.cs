using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Portfolio;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Entities.Dto;

namespace Mandara.Business.TradeAdd
{
    public class TradeAddDetailsDto
    {
        public PortfolioDto Portfolio1 { get; set; }
        public PortfolioDto Portfolio2 { get; set; }

        public string Broker { get; set; }
        public string Exchange { get; set; }

        public TradeTypeControl TradeType { get; set; }
        public SideControl Side { get; set; }
        public StripTypeControl StripTypeControl { get; set; }

        public StripDetail StripDetail1 { get; set; }
        public StripDetail StripDetail2 { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedByUserName { get; set; }

        public ProductDto3 Product { get; set; }
        public int OfficialProductId { get; set; }

        public List<int> TradeCaptureIds { get; set; }
        public int? GroupId { get; set; }
        public string EditCancelReason { get; set; }

        public string ExpiryExchange { get; set; }
        public bool? IsTasChecked { get; set; }
        public bool? IsMopsChecked { get; set; }
        public bool? IsMmChecked { get; set; }
        public bool? IsMocChecked { get; set; }
        public bool IsMasterToolMode { get; set; }
        public DateTime? TradeDate { get; set; }
        public DateTime? TimestampUtc { get; set; }
        public DateTime? TransactTimeUtc { get; set; }

        public DateTime ForwardValueDate { get; set; }

        public decimal SpecifiedAmount { get; set; }

        public decimal AgainstAmount { get; set; }

        public decimal FxExchangeRate { get; set; }

        public bool IsSpot { get; set; }

        public Instrument FxSelectedInstrument { get; set; }
    }
}