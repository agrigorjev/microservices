using Mandara.Entities.Enums;
using Mandara.Entities.ErrorDetails;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace Mandara.Entities
{
    [Table("trade_transfer_errors")]
    public partial class TradeTransferError
    {
        /// <summary>
        /// Entity Framework requires a parameterless constructor.
        /// </summary>
        private TradeTransferError()
        {
        }

        public TradeTransferError(string errorMessage)
        {
            if (String.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(
                    String.Format("TradeTransferError constuction: {0} cannot be empty or null.", nameof(errorMessage)));
            }

            ErrorMessage = errorMessage;
        }

        public TradeTransferError(ExceptionDetails exceptionDetails)
        {
            if (null == exceptionDetails)
            {
                throw new ArgumentNullException("TradeTransferError construction: ExceptionDetails cannot be null.");
            }

            ExceptionDetail = exceptionDetails;
        }


        [Key]
        [Column("entity_id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EntityId { get; set; }

        [Key]
        [Column("entity_type", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short EntityTypeDb { get; set; }

        [Column("error_date")]
        public DateTime ErrorDate { get; set; }

        [Column("error_type")]
        public short ErrorTypeDb { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }


        [NotMapped]
        public TransferErrorType ErrorType
        {
            get
            {
                return (TransferErrorType)ErrorTypeDb;
            }
            set
            {
                ErrorTypeDb = (Int16)value;
            }
        }

        [NotMapped]
        public TransferEntityType EntityType
        {
            get
            {
                return (TransferEntityType)EntityTypeDb;
            }
            set
            {
                EntityTypeDb = (Int16)value;
            }
        }

        private ExceptionDetails _exceptionDetail;

        [NotMapped]
        public ExceptionDetails ExceptionDetail
        {
            get
            {
                if (_exceptionDetail == null)
                    _exceptionDetail = ConvertExceptionFromString(ErrorMessage);

                return _exceptionDetail;
            }
            set
            {
                ErrorMessage = value != null ? ConvertExceptionToString(value) : null;

                _exceptionDetail = value;
            }
        }

        public string ConvertExceptionToString(ExceptionDetails ex)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExceptionDetails));

            using (var ms = new MemoryStream())
            {
                using (var gzStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    xmlSerializer.Serialize(gzStream, ex);
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public ExceptionDetails ConvertExceptionFromString(string str)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExceptionDetails));

            using (var ms = new MemoryStream(Convert.FromBase64String(str)))
            {
                ms.Seek(0, SeekOrigin.Begin);

                using (var gzStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    return (ExceptionDetails)xmlSerializer.Deserialize(gzStream);
                }
            }
        }
    }

    public class TradeTransferErrorDetails
    {
        public int? ProductId { get; set; }
        public string Message { get; set; }
        public string Product { get; set; }
        public string Exchange { get; set; }
        public string UnderlyingSecurityID { get; set; }
        public string UnderlyingSecurityIDSource { get; set; }
    }

}
