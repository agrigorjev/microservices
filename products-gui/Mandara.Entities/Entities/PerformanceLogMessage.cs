using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("_performance_log")]
    public partial class PerformanceLogMessage
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("TradeCaptureId")]
        public int TradeCaptureId { get; set; }

        [Column("FeedHandlerRead")]
        public DateTime? FeedHandlerRead { get; set; }

        [Column("TransferServiceTransferred")]
        public DateTime? TransferServiceTransferred { get; set; }

        [Column("IrmServerRead")]
        public DateTime? IrmServerRead { get; set; }

        [Column("IrmServerPositionCalculated")]
        public DateTime? IrmServerPositionCalculated { get; set; }

        [Column("IrmServerPositionUpdateMessageSent")]
        public DateTime? IrmServerPositionUpdateMessageSent { get; set; }
    }
}
