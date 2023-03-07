using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Mandara.Entities.Services.DayPositionsSerialisation;

namespace Mandara.Entities
{
    [Table("precalc_details_sd")]
    public class PrecalcSdDetail : PrecalcDetail, INewable
    {
        [Key]
        [Column("PrecalcDetailId")]
        public int PrecalcDetailId { get; set; }

        [Column("Month")]
        public override DateTime Month { get; set; }

        [Column("DaysSerialized")]
        public string DaysSerialized
        {
            get => _daysSerialized;
            set
            {
                _daysSerialized = value;
                daysPositions = DeserializeDaysNullablePositions(
                        value,
                        Month,
                        ProductId,
                        "security definition",
                        SecurityDefinitionId);

                dailyPositions = BuildDailyPositions(daysPositions);
            }
        }

        [Column("SecurityDefinitionId")]
        public int SecurityDefinitionId { get; set; }

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

        [ForeignKey("SecurityDefinitionId")]
        public virtual SecurityDefinition SecurityDefinition
        {
            get => _securityDefinition;
            set
            {
                _securityDefinition = value;
                SecurityDefinitionId = _securityDefinition?.SecurityDefinitionId ?? 0;
            }
        }


        private Product _product;
        private SecurityDefinition _securityDefinition;
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
                    "security definition",
                    SecurityDefinitionId);

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
