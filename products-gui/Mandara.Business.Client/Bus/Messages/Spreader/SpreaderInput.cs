using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Spreader
{
    public class SpreaderInput: IComparer<SpreaderInput>
    {
        public DateTime Month { get; set; }
        public ProductDateType DateType { get; set; }
        public decimal? SwapAmount { get; set; }
        public decimal? FuturesAmount { get; set; }

        public int Compare(SpreaderInput x, SpreaderInput y)
        {
            return x.Month.CompareTo(y.Month);
        }
    }
}
