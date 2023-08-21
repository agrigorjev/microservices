using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;

namespace Mandara.TradeApiService.GrpcDefinitions
{
    public partial class TradeAddDetailsGrpc
    {
        private bool HasExactlyOneQOrCalStrip()
        {
            return IsStripAQOrCal(StripDetail1.Strip) ^ IsStripAQOrCal(StripDetail2.Strip);
        }

        private bool IsStripAQOrCal(StripGrpc strip)
        {
            return strip.StringValue.Contains("Q") || strip.StringValue.Contains("Cal");
        }

        public bool SpreadHasABalmoStrip()
        {
            return StripDetail1.Strip.IsBalmoStrip || StripDetail2.Strip.IsBalmoStrip;
        }

        public bool IsSpreadWithOneQOrCalStrip()
        {
            return null != StripDetail2 && HasExactlyOneQOrCalStrip();
        }
    }
}
