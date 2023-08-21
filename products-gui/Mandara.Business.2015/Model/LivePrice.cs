using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Entities;

namespace Mandara.Business.Model
{
    public class LivePrice
    {
        public Money? TradeLivePrice { get; set; }
        public Money? Leg1LivePrice { get; set; }
        public Money? Leg2LivePrice { get; set; }
    }
}
