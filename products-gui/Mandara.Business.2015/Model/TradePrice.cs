using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Entities;

namespace Mandara.Business.Model
{
    public class TradePrice
    {
        public Money? TradedPrice { get; set; }
        public Money? Leg1Price { get; set; }
        public Money? Leg2Price { get; set; }
    }
}
