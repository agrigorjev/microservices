using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("vhal_messages")]
    public partial class VHALMessage
    {
        private Broker _broker;

        public VHALMessage()
        {
            this.HALProducts = new HashSet<HALProduct>();
        }

        [Column("message_id")]
        [Key]
        public long MessageId { get; set; }

        [Column("group_name")]
        [StringLength(160)]
        public string GroupName { get; set; }

        [Column("raw_text")]
        [StringLength(2000)]
        public string RawText { get; set; }

        [Column("message_text")]
        [StringLength(2000)]
        public string MessageBody { get; set; }

        [Column("timestamp_received")]
        public DateTime? Received { get; set; }

        [Column("broker_id")]
        public int? BrokerId { get; set; }

        [Column("channel_name")]
        [StringLength(50)]
        public string ChannelName { get; set; }

        [Column("voice_file_name")]
        [StringLength(255)]
        public string VoiceFileName { get; set; }

        [ForeignKey("BrokerId")]
        public virtual Broker Broker
        {
            get { return _broker; }
            set
            {
                _broker = value;
                BrokerId = _broker != null ? _broker.BrokerId : (int?) null;
            }
        }

        public virtual ICollection<HALProduct> HALProducts { get; set; }
    }
}
