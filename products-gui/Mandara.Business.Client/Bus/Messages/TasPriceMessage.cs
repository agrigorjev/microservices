using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus.Messages
{
    public class TasPriceMessage : SnapshotMessageBase
    {
        public List<TasPriceView> TasPriceData { get; set; }
        public bool ZeroOnly { get; set; }
        public List<PriceChanges> changedPrices;
    }
    public struct PriceChanges
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int User { get; set; }
    }
}
