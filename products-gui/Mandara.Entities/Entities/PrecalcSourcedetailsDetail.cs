using Mandara.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("precalc_details_sourcedetails")]
    public class PrecalcSourcedetailsDetail : PrecalcDetail
    {
        [Key]
        [Column("PrecalcDetailId")]
        public int PrecalcDetailId { get; set; }

        [Column("Month")]
        public DateTime Month { get; set; }

        [Column("DaysSerialized")]
        public string DaysSerialized { get; set; }

        [Column("SourceDetailId")]
        public int SourceDetailId { get; set; }

        [Column("ProductId")]
        public int ProductId { get; set; }

        [Column("MinDay")]
        public DateTime? MinDay { get; set; }

        [Column("MaxDay")]
        public DateTime? MaxDay { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product
        {
            get =>_product;
            set
            {
                _product = value;
                ProductId = _product?.ProductId ?? 0;
            }
        }

        [ForeignKey("SourceDetailId")]
        public virtual SourceDetail SourceDetail
        {
            get => _sourceDetail;
            set
            {
                _sourceDetail = value;
                SourceDetailId = _sourceDetail?.SourceDetailId ?? 0;
            }
        }

        private Product _product;
        private SourceDetail _sourceDetail;

        [NotMapped]
        public Dictionary<DateTime, decimal?> DaysPositions
        {
            get
            {
                if (daysPositions != null)
                {
                    return daysPositions;
                }

                daysPositions = DayPositionsSerialisation.DeserializeDaysNullablePositions(
                    DaysSerialized,
                    Month,
                    ProductId,
                    "source detail",
                    SourceDetailId);

                dailyPositions = BuildDailyPositions(daysPositions);
                return daysPositions;
            }
            set
            {
                daysPositions = value ?? new Dictionary<DateTime, decimal?>();
                DaysSerialized =
                    DayPositionsSerialisation.SerializeDaysPositions(daysPositions);

                dailyPositions = BuildDailyPositions(daysPositions);
            }
        }
    }
}
