using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("test_pnl_bookpnl")]
    public partial class TestPnlBookpnl
    {
        private TestPnlSnapshot _testPnlSnapshot;

        public TestPnlBookpnl()
        {
            this.TestPnlTradepnls = new HashSet<TestPnlTradepnl>();
        }

        [Column("snapshot_id")]
        public int SnapshotId { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("live_pnl")]
        public decimal LivePnl { get; set; }

        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("SnapshotId")]
        public virtual TestPnlSnapshot TestPnlSnapshot
        {
            get { return _testPnlSnapshot; }
            set
            {
                _testPnlSnapshot = value;
                SnapshotId = _testPnlSnapshot != null ? _testPnlSnapshot.Id : 0;
            }
        }

        public virtual ICollection<TestPnlTradepnl> TestPnlTradepnls { get; set; }

    }
}
