namespace Mandara.Entities.Calculation
{
    public class RolloffDetail
    {
        public static readonly RolloffDetail Default = new RolloffDetail
        {
            CategoryName = string.Empty,
            PortfolioName = string.Empty,
            SourceProductName = string.Empty,
            ProductName = string.Empty,
            OldAmount = 0M,
            NewAmount = 0M
        };

        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string SourceProductName { get; set; }
        public string PortfolioName { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }

        public bool IsDefault()
        {
            return this == Default
                   || (CategoryName == Default.CategoryName
                       && ProductName == Default.ProductName
                       && SourceProductName == Default.SourceProductName
                       && PortfolioName == Default.PortfolioName
                       && OldAmount == Default.OldAmount
                       && NewAmount == Default.NewAmount);
        }
    }
}