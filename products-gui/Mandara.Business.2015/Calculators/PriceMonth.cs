using System;
using Mandara.Date;

namespace Mandara.Business.Calculators
{
    public static class PriceMonth
    {
        public static int Get(DateTime from, DateTime to)
        {
            return to.MonthsSince(from) + 72;
        }
    }
}