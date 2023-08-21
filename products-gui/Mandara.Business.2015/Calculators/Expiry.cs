using Mandara.Business.OldCode;
using System;

namespace Mandara.Business.Calculators
{
    public class Expiry
    {
        public DateTime Expires { get; }
        public CalendarFailure FailureReason { get; }


        public Expiry(DateTime expires)
        {
            Expires = expires;
            FailureReason = CalendarFailure.None;
        }

        public Expiry(CalendarFailure failure)
        {
            Expires = DateTime.MinValue;
            FailureReason = failure;
        }

        public bool HasDate() => CalendarFailure.None == FailureReason;
    }
}
