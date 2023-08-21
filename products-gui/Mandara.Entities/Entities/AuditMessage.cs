using Google.Protobuf;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;

namespace Mandara.Entities
{
    [Table("audit_message")]
    public class AuditMessage : INewable
    {
        private User _user;

        [Column("audit_message_id")]
        [Key]
        public int AuditMessageId { get; set; }

        [Column("message_type")]
        [StringLength(255)]
        public string MessageType { get; set; }

        [Column("message_time")]
        public DateTime MessageTime { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [Column("object_type")]
        [StringLength(255)]
        public string ObjectType { get; set; }

        [Column("object_id")]
        public int? ObjectId { get; set; }

        [Column("object_description")]
        public string ObjectDescription { get; set; }

        [Column("message_details")]
        public string MessageDetails { get; set; }

        [Column("source")]
        [StringLength(255)]
        public string Source { get; set; }

        [Column("context_id")]
        [StringLength(255)]
        public string ContextId { get; set; }

        [Column("context_name")]
        [StringLength(255)]
        public string ContextName { get; set; }

        [Column("book_name")]
        [StringLength(255)]
        public string BookName { get; set; }

        [Column("user_ip")]
        [StringLength(255)]
        public string UserIp { get; set; }

        [Column("user_name")]
        [StringLength(255)]
        public string UserName { get; set; }

        [ForeignKey("user_id")]
        public virtual User User
        {
            get { return _user; }
            set
            {
                _user = value;
                user_id = _user != null ? _user.UserId : (int?)null;
            }
        }

        [NotMapped]
        public string UserNameOrRef
        {
            get
            {
                if (User != null)
                    return User.UserName;

                return UserName;
            }
        }

        [NotMapped]
        public AuditMessageDetails Details { get; set; }


        public bool IsNew()
        {
            return 0 == AuditMessageId;
        }
    }
}
