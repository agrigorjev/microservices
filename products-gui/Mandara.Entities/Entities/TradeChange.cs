using Mandara.Entities.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    [Table("trade_changes")]
    public partial class TradeChange: INewable
    {
        [NotMapped]
        public const TradeChangeEntityType DefaultTradeChangeEntityType = TradeChangeEntityType.TradeCapture;

        private TradeCapture _tradeCapture;

        public TradeChange()
        {
            EntityType = DefaultTradeChangeEntityType;
        }

        [Key]
        [Column("change_id")]
        public int ChangeId { get; set; }

        [Column("trade_id")]
        public int TradeId { get; set; }

        [Column("from_portfolio_id")]
        public int? FromPortfolioId { get; set; }

        [Column("trade_change_type")]
        public short TradeChangeTypeDb { get; set; }

        [Column("change_date")]
        public DateTime ChangeDate { get; set; }

        [Column("old_quantity")]
        public decimal? OldQuantity { get; set; }

        [ForeignKey("TradeId")]
        public virtual TradeCapture TradeCapture
        {
            get { return _tradeCapture; }
            set
            {
                _tradeCapture = value;
                TradeId = _tradeCapture != null ? _tradeCapture.TradeId : 0;
            }
        }

        [NotMapped]
        private int _entityTypeDbValue;

        [Column("entity_type")]
        public int EntityTypeDb
        {
            get { return _entityTypeDbValue; }
            set
            {
                SetEntityTypeDbValue(value);
            }
        }

        private void SetEntityTypeDbValue(int entityTypeDbValue)
        {
            if (!Enum.IsDefined(typeof(TradeChangeEntityType), entityTypeDbValue))
            {
                throw new InvalidEnumArgumentException(
                    String.Format("{0} is not a valid TradeChangeEntityType value.", entityTypeDbValue));
            }

            _entityTypeDbValue = entityTypeDbValue;
        }

        [NotMapped]
        public TradeChangeEntityType EntityType
        {
            get
            {
                return (TradeChangeEntityType)EntityTypeDb;
            }
            set
            {
                SetEntityTypeDbValue((int)value);
            }
        }

        [NotMapped]
        public TradeChangeType TradeChangeType
        {
            get
            {
                return (TradeChangeType)TradeChangeTypeDb;
            }
            set
            {
                TradeChangeTypeDb = (Int16)value;
            }
        }

        public static TradeChange Create(
            TradeCapture tradeCapture,
            DateTime timestamp,
            TradeChangeType changeTp,
            TradeChangeEntityType entityTp)
        {
            return new TradeChange
            {
                TradeCapture = tradeCapture,
                TradeChangeType = changeTp,
                ChangeDate = timestamp,
                FromPortfolioId = tradeCapture.PortfolioId,
                EntityType = entityTp
            };
        }

        public bool IsNew()
        {
            return ChangeId == 0;
        }
    }

    public enum TradeChangeEntityType
    {
        TradeCapture = 1,
        FxTrade = 2
    }

    [NotMapped]
    public class FxTradeChange : TradeChange
    {
        public FxTrade FxTrade { get; set; }
    }
}
