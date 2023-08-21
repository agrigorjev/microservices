using System;
using System.Collections.Generic;
using Mandara.Entities;

namespace Mandara.Business.Model
{
    public class TradeModel
    {
        public int TradeId { get; set; }
        public int SecurityDefinitionId { get; set; }
        public int? PortfolioId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? TradeStartDate { get; set; }
        public DateTime? TradeEndDate { get; set; }
        public DateTime? TradeDate { get; set; }
        public List<PrecalcDetailModel> PrecalcDetails { get; set; }
        public List<TradeChange> TradeChanges { get; set; }
        public SecurityDefinition SecurityDefinition { get; set; }
        public bool? IsParentTrade { get; set; }
    }
}