using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandara.Entities.Enums;

namespace Mandara.Entities
{
    [Table("fix_messages")]
    public class FixMessage
    {
        [Key]
        [Column("fix_message_id")]
        public int FixMessageId { get; set; }

        [Column("exchange")]
        [Required]
        [StringLength(5)]
        public string Exchange { get; set; }

        [Column("message_date")]
        public DateTime MessageDate { get; set; }

        [Column("sender")]
        [Required]
        [StringLength(20)]
        public string Sender { get; set; }

        [Column("message_type")]
        [Required]
        [StringLength(30)]
        public string MessageType { get; set; }

        [Column("request_status")]
        [StringLength(10)]
        public string RequestStatus { get; set; }

        [Column("message_body")]
        public string MessageBody { get; set; }



        [NotMapped]
        public FixMessageType MessageTypeEnum
        {
            get { return (FixMessageType)Enum.Parse(typeof(FixMessageType), MessageType); }
            set { MessageType = value.ToString(); }
        }

        [NotMapped]
        public FixRequestStatus RequestStatusEnum
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(RequestStatus))
                {
                    return (FixRequestStatus)Enum.Parse(typeof(FixRequestStatus), RequestStatus);
                }
                return FixRequestStatus.Unknown;
            }
            set
            {
                RequestStatus = value != FixRequestStatus.Unknown ? value.ToString() : null;
            }
        }

    }
}
