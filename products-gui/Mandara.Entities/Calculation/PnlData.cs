namespace Mandara.Entities.Calculation
{
    // TODO: Give this class a meaningful name, e.g. PnLAndCosts
    // TODO: Don't allow nullable Money.
    public class PnlData
    {
        public Money? LivePnl { get; set; }
        public Money? OvernightPnl { get; set; }
        public Money? LiveCosts { get; set; }
        /// <summary>
        /// According IRM-379 we need transfer brokerage too
        /// </summary>
        public Money? Brokerage { get; set; }

        public bool IsEmpty
        {
            get { return IsZero(LivePnl) && IsZero(OvernightPnl) && IsZero(LiveCosts) && IsZero(Brokerage); }
        }

        private bool IsZero(Money? pnl)
        {
            return !pnl.HasValue || pnl.Value.IsZero();
        }

        public Money NetPnl
        {
            get { return GetValue(LivePnl) + GetValue(OvernightPnl) - GetValue(LiveCosts) - GetValue(Brokerage); }
        }

        private Money GetValue(Money? value)
        {
            return value ?? new Money(0, "");
        }
    }
}