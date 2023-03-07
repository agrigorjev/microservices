using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Mandara.Entities.Calculation
{
    public class NotMappedProduct
    {
        public String RowNumber { get; set; }
        public String Message { get; set; }
        public int TradeId { get; set; }
        public DataRow TradeRow { get; set; }
    }
}
