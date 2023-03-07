using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("fx_pairs")]
    public class FxPair
    {
        private Currency _fromCurrency;
        private Currency _toCurrency;
        private int _fromCurrencyId;
        private int _toCurrencyId;
        private string _name;

        [Column("pair_id")]
        [Key]
        public int FxPairId { get; set; }

        [Column("from_currency_id")]
        public int FromCurrencyId
        {
            get
            {
                return _fromCurrencyId;
            }
            set
            {
                _fromCurrencyId = value;
            }
        }

        [Column("to_currency_id")]
        public int ToCurrencyId
        {
            get
            {
                return _toCurrencyId;
            }
            set
            {
                _toCurrencyId = value;
            }
        }

        [Column("pair_name")]
        public string PairName
        {
            get
            {
                return ToString();
            }
            set
            {
                _name = value;
            }
        }

        [ForeignKey("Currency1Id")]
        public Currency FromCurrency
        {
            get
            {
                return _fromCurrency;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("A CurrencyPair requires both currencies.");
                }

                FromCurrencyId = value.CurrencyId;
                _fromCurrency = value;
            }
        }

        [ForeignKey("Currency2Id")]
        public Currency ToCurrency
        {
            get
            {
                return _toCurrency;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException("A CurrencyPair requires both currencies.");
                }

                ToCurrencyId = value.CurrencyId;
                _toCurrency = value;
            }
        }

        private void UpdateStringRepresentation()
        {
            if (FromCurrency == null || ToCurrency == null)
            {
                return;
            }

            _name = string.Format("{0}/{1}", FromCurrency.IsoName, ToCurrency.IsoName);
        }

        public override string ToString()
        {
            if (_name == null)
            {
                UpdateStringRepresentation();
            }

            return _name;
        }
    }
}
