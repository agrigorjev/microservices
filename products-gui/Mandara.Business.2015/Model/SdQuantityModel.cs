namespace Mandara.Business.Model
{
    public class SdQuantityModel
    {
        public int SecurityDefinitionId { get; set; }
        public int PortfolioId { get; set; }
        public decimal TradesQuantity { get; set; }
        public decimal TradesFxExposure { get; set; }
    }
}