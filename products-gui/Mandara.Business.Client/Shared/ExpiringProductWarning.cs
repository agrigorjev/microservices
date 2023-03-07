using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business
{
    public class ExpiringProductWarning
    {
        public long SecurityDefinitionId { get; set; }
        public string ProductDescription { get; set; }
        public string StripName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Quantity { get; set; }

        private string _markAsDelivered = "Mark as delivered";
        public string MarkAsDelivered
        {
            get { return _markAsDelivered; }
            set { _markAsDelivered = value; }
        }
    }
}
