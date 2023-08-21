using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("foreign_currency_position_details")]
    public class ForeignCurrencyPositionDetail
    {
        private TradeCapture _tradeCapture;
        private ForeignCurrencyPosition _foreignCurrencyPosition;

        [Key]
        [Column("foreign_currency_position_detail_id")]
        public int ForeignCurrencyPositionDetailId { get; set; }

        [Column("foreign_currency_position_id")]
        public int ForeignCurrencyPositionId { get; set; }

        [Column("trade_capture_id")]
        public int TradeCaptureId { get; set; }

        [Column("value")]
        public decimal PositionValue { get; set; }

        [ForeignKey("TradeCaptureId")]
        public TradeCapture TradeCapture
        {
            get { return _tradeCapture; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _tradeCapture = value;
                TradeCaptureId = _tradeCapture.TradeId;
            }
        }

        [ForeignKey("ForeignCurrencyPositionId")]
        public ForeignCurrencyPosition ForeignCurrencyPosition
        {
            get { return _foreignCurrencyPosition; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _foreignCurrencyPosition = value;
                ForeignCurrencyPositionId = _foreignCurrencyPosition.ForeignCurrencyPositionId;
            }
        }

    }
}
