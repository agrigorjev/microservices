using System;
using System.Text;

namespace Mandara.Entities.Calculation
{
    public class CalculationError
    {
        public String ErrorMessage { get; set; }
        public SourceDetail SourceDetail { get; set; }
        public String UserName { get; set; }

        public override string ToString()
        {
            StringBuilder calcErrMsg = new StringBuilder();

            calcErrMsg.Append(ErrorMessage);

            if (null != SourceDetail)
            {
                calcErrMsg.AppendFormat(
                    "\nTrade details: {0} {1} {2} at {3} for {4}",
                    SourceDetail.BuySell,
                    SourceDetail.Quantity ?? 0,
                    SourceDetail.SecurityDefinition.ProductName,
                    SourceDetail.Price,
                    SourceDetail.StripName);
            }

            if (null != UserName)
            {
                calcErrMsg.AppendFormat("\nby {0}", UserName);
            }

            return calcErrMsg.ToString();
        }
    }
}
