using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Mandara.Entities.Trades;

namespace Mandara.Entities.ErrorDetails
{
    public class TasCheckErrorDetail : ErrorDetails
    {
        [Category("TAS Check")]
        [Display(Order = 1)]
        public string Category { get; set; }

        [Category("TAS Check")]
        [Display(Order = 2, Name = "Expected quantity")]
        [DisplayName("Expected quantity")]
        public string ExpectedQuantity { get; set; }

        [Category("TAS Check")]
        [Display(Order = 3, Name = "Actual quantity")]
        [DisplayName("Actual quantity")]
        public string ActualQuantity { get; set; }

        public TasCheckErrorDetail(TasCheckDetail tasCheckDetail)
        {
            if (tasCheckDetail != null)
            {
                Category = tasCheckDetail.CategoryName;
                ExpectedQuantity = tasCheckDetail.ExpectedQuantity.ToString("f2");
                ActualQuantity = tasCheckDetail.RealQuantity.ToString("f2");
            }
        }
    }
}