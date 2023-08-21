using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mandara.TradeApiService.Data
{
    [Table("trade_groups")]
    public partial class TradeGroup
    {
        public TradeGroup()
        {
            this.TradeCaptures = new HashSet<TradeCapture>();
        }

        [Column("group_id")]
        [Key]
        public int GroupId { get; set; }

        public virtual ICollection<TradeCapture> TradeCaptures { get; set; }

        public override string ToString()
        {
            return GroupId.ToString();
        }

    }

}
