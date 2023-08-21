using System;

namespace Mandara.Entities.Dto
{
    public class FxTradeDto
    {
        public int FxTradeId { get; set; }

        public string ProductType { get; set; }

        public decimal SpecifiedAmount { get; set; }

        public decimal AgainstAmount { get; set; }

        public decimal Rate { get; set; }

        public decimal SpotRate { get; set; }

        public string Tenor { get; set; }

        public DateTime ValueDate { get; set; }

        public ProductDto FxProduct;

        public TradeCaptureDto Trade;

        public virtual Currency AgainstCurrency { get; set; }

        public virtual Currency SpecifiedCurrency { get; set; }
    }
}
