using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("test_pnl_snapshot")]
    public partial class TestPnlSnapshot
    {
        public TestPnlSnapshot()
        {
            this.TestPnlBookpnls = new HashSet<TestPnlBookpnl>();
        }

        [Column("id")]
        public int Id { get; set; }

        [Column("pnl_date")]
        public DateTime PnlDate { get; set; }

        public virtual ICollection<TestPnlBookpnl> TestPnlBookpnls { get; set; }

    }
}
