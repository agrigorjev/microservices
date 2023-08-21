using System;
using System.ComponentModel;

namespace Mandara.Data
{
    public enum TradeCaptureSide
    {
        [Description("Sell")]
        Sell,
        [Description("Buy")]
        Buy
    }

    public static class TradeSide
    {
        public const string Sell = "Sell";
        public const string Buy = "Buy";

        public static bool IsBuy(string side)
        {
            return Buy.Equals(side, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsSell(string side)
        {
            return Sell.Equals(side, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}