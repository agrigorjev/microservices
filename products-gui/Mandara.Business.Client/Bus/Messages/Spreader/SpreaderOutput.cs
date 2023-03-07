using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Business.Bus.Messages.Spreader
{
    public class SpreaderOutput
    {
        public DateTime Month { get; set; }
        public decimal? FuturesEquivalent { get; set; }
        public decimal JALSpreads { get; set; }
        public decimal BookFuturesEquivalent { get; set; }
        public decimal TotalJALSpreads { get; set; }

        public SpreaderOutput Clone()
        {
            return new SpreaderOutput()
            {
                Month = Month,
                FuturesEquivalent = FuturesEquivalent,
                BookFuturesEquivalent = BookFuturesEquivalent,
                JALSpreads = JALSpreads,
                TotalJALSpreads = TotalJALSpreads
            };
        }
    }
}
