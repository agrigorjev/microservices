using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("test_pnl_tradepnl")]
    public partial class TestPnlTradepnl
    {
        private TestPnlBookpnl _testPnlBookpnl;

        [Key]
        [Column("bookpnl_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BookpnlId { get; set; }

        [Key]
        [Column("trade_id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TradeId { get; set; }

        [Column("live_price")]
        public decimal LivePrice { get; set; }

        [Column("live_pnl")]
        public decimal LivePnl { get; set; }

        [ForeignKey("BookpnlId")]
        public virtual TestPnlBookpnl TestPnlBookpnl
        {
            get { return _testPnlBookpnl; }
            set
            {
                _testPnlBookpnl = value;
                if (_testPnlBookpnl != null)
                    BookpnlId = _testPnlBookpnl.Id;
            }
        }
    }
}
