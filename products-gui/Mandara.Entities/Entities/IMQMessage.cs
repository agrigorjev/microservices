using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("imq_messages")]
    public partial class IMQMessage
    {
        private Broker _broker;

        public IMQMessage()
        {
            ParsedProducts = new HashSet<IMQQuote>();
        }

        [Column("imq_message_id")]
        [Key]
        public int MessageId { get; set; }

        [Column("group_name")]
        [StringLength(160)]
        public string GroupName { get; set; }

        [Column("timestamp_recieved")]
        public DateTime? Received { get; set; }

        [Column("message_text")]
        [StringLength(2000)]
        public string MessageBody { get; set; }

        [Column("broker_id")]
        public int BrokerId { get; set; }

        [Column("channel_name")]
        [StringLength(50)]
        public string channel_name { get; set; }

        [Column("voice_file_name")]
        [StringLength(255)]
        public string voice_file_name { get; set; }

        [Column("message_type")]
        public int? message_type { get; set; }

        [ForeignKey("BrokerId")]
        public virtual Broker Broker
        {
            get { return _broker; }
            set
            {
                _broker = value;
                BrokerId = _broker != null ? _broker.BrokerId : 0;
            }
        }

        public virtual ICollection<IMQQuote> ParsedProducts { get; set; }
    }
}
