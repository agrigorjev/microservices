using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Business.Bus.Messages
{
    using Mandara.Business.Bus.Messages.Base;

    public class ExpiringProductsSnapshotMessage : SnapshotMessageBase
    {
        public int DaysToExpire { get; set; }
        public List<ExpiringProductWarning> ExpiringProducts { get; set; }

        public ExpiringProductsSnapshotMessage()
        {
        }

        public ExpiringProductsSnapshotMessage(int daysToExpire)
        {
            DaysToExpire = daysToExpire;
        }
    }
}
