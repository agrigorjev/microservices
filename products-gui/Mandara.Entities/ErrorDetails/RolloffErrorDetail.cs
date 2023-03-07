using System.ComponentModel;
using Mandara.Entities.Calculation;

namespace Mandara.Entities.ErrorDetails
{
    public class RolloffErrorDetail : ErrorDetails
    {
        [Category("Rolloff")]
        public string Category { get; set; }

        [Category("Rolloff")]
        public string Product { get; set; }

        [Category("Rolloff")]
        public string Source { get; set; }

        [Category("Rolloff")]
        public string Book { get; set; }

        [Category("Rolloff")]
        [DisplayName("Old Amount")]
        public string OldAmount { get; set; }

        [Category("Rolloff")]
        [DisplayName("New Amount")]
        public string NewAmount { get; set; }

        public RolloffErrorDetail(RolloffDetail detail)
        {
            Category = detail.CategoryName;
            Product = detail.ProductName;
            Source = detail.SourceProductName;
            Book = detail.PortfolioName;
            OldAmount = detail.OldAmount.ToString("f2");
            NewAmount = detail.NewAmount.ToString("f2");
        }
    }
}