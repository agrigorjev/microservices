using Mandara.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Mandara.Entities.Services.DayPositionsSerialisation;

namespace Mandara.Entities
{
    [Table("precalc_details_tc")]
    public class PrecalcTcDetail : PrecalcDetail, INewable
    {
        [Key]
        [Column("PrecalcDetailId")]
        public int PrecalcDetailId { get; set; }

        [Column("Month")]
        public override DateTime Month { get; set; }

        [Column("DaysSerialized")]
        public string DaysSerialized
        {
            get =>  _daysSerialized;
            set
            {
                _daysSerialized = value;
                daysPositions = DeserializeDaysNullablePositions(
                        value,
                        Month,
                        ProductId,
                        "trade",
                        TradeCaptureId);

                dailyPositions = BuildDailyPositions(daysPositions);
            }
        }

        [Column("TradeCaptureId")]
        public int TradeCaptureId { get; set; }

        [Column("ProductId")]
        public override int ProductId { get; set; }

        [Column("MinDay")]
        public override DateTime MinDay { get; set; }

        [Column("MaxDay")]
        public override DateTime MaxDay { get; set; }

        [ForeignKey("ProductId")]
        public override Product Product
        {
            get => _product;
            set
            {
                _product = value;
                ProductId = _product?.ProductId ?? 0;
            }
        }

        [ForeignKey("TradeCaptureId")]
        public virtual TradeCapture TradeCapture
        {
            get => _tradeCapture;
            set
            {
                _tradeCapture = value;
                TradeCaptureId = _tradeCapture?.TradeId ?? 0;
            }
        }


        private Product _product;
        private TradeCapture _tradeCapture;
        private string _daysSerialized;

        [NotMapped]
        [Obsolete("Just use the base class property")]
        public override Dictionary<DateTime, decimal?> DaysPositions
        {
            get
            {
                if (daysPositions != null)
                {
                    return daysPositions;
                }

                daysPositions = DeserializeDaysNullablePositions(
                    DaysSerialized,
                    Month,
                    ProductId,
                    "trade",
                    TradeCaptureId);

                dailyPositions = BuildDailyPositions(daysPositions);
                return daysPositions;
            }
            set
            {
                daysPositions = value ?? new Dictionary<DateTime, decimal?>();
                _daysSerialized = SerializeDaysPositions(daysPositions);

                dailyPositions = BuildDailyPositions(daysPositions);
            }
        }

        public bool IsNew()
        {
            return 0 == PrecalcDetailId;
        }
    }
}
