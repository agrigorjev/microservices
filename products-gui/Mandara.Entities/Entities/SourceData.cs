using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Mandara.Entities
{
    [Table("source_data_info")]
    public partial class SourceData
    {
        public SourceData()
        {
            SourceDetails = new HashSet<SourceDetail>();
            SealDetails = new HashSet<SealDetail>();
        }

        [Column("source_data_id")]
        [Key]
        public int SourceDataId { get; set; }

        [Column("source_data_type")]
        public short TypeDb { get; set; }

        [Column("effective_date", TypeName = "date")]
        public DateTime Date { get; set; }

        [Column("imported_datetime")]
        public DateTime? ImportedDateTime { get; set; }

        public virtual ICollection<SourceDetail> SourceDetails { get; set; }

        public virtual ICollection<SealDetail> SealDetails { get; set; }


        [NotMapped]
        public SourceDataType Type
        {
            get
            {
                return (SourceDataType)TypeDb;
            }
            set
            {
                TypeDb = (Int16)value;
            }
        }

        [NotMapped]
        [Description("Source Data Type")]
        [DataMember]
        public string SourceDataTypeString
        {
            get
            {
                switch (Type)
                {
                    case SourceDataType.OpenPositions:
                        return "Open Positions";
                    case SourceDataType.TradeActivity:
                        return "Trade Activity";
                    case SourceDataType.Seals:
                        return "Seals Import";
                    default:
                        return "";
                }
            }
        }

        public const int NewSourceDataId = -1;

        public bool IsNew()
        {
            return NewSourceDataId == SourceDataId;

        }

        private string _fileName;

        [NotMapped]
        [DataMember]
        [Description("Source Data File")]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public override string ToString()
        {
            return string.Format(
                "Source Data Type: {0}, Source Date: {1}, Source Data File: {2}",
                SourceDataTypeString,
                Date,
                FileName);
        }

        private static readonly DateTime DefaultDate = DateTime.MinValue;
        private const string DefaultFile = "NeverImported";
        private const int DefaultId = 0;
        private const SourceDataType DefaultDataType = SourceDataType.TradeActivity;

        public static readonly SourceData Default = new SourceData()
        {
            Date = DefaultDate,
            FileName = DefaultFile,
            ImportedDateTime = DefaultDate,
            SourceDataId = DefaultId,
            SealDetails = new List<SealDetail>(),
            SourceDetails = new List<SourceDetail>(),
            Type = DefaultDataType,
        };

        public bool IsDefault()
        {
            return DefaultDate == Date
                   && DefaultFile == FileName
                   && DefaultDate == ImportedDateTime
                   && DefaultId == SourceDataId
                   && DefaultDataType == Type;
        }
    }
}
