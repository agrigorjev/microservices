using System;

namespace Mandara.Entities
{
    public class CurrencyPair
    {
        private Currency _fromCurrency;
        private Currency _toCurrency;
        private string _name;
        private string _hashCodeStringRepresentation;

        private const int DefaultId = -1;
        private const int DefaultFirstCurrId = -1;
        private const string DefaultFirstCurrName = "CR1";
        private const int DefaultSecondCurrId = -2;
        private const string DefaultSecondCurrName = "CR2";
        public static CurrencyPair Default = new CurrencyPair(
            DefaultId,
            new Currency { CurrencyId = DefaultFirstCurrId, IsoName = DefaultFirstCurrName },
            new Currency { CurrencyId = DefaultSecondCurrId, IsoName = DefaultSecondCurrName });

        public bool IsDefault()
        {
            return null != _fromCurrency
                   && null != _toCurrency
                   && (Object.ReferenceEquals(this, Default)
                       || (DefaultId == CurrencyPairId
                           && DefaultFirstCurrId == _fromCurrency.CurrencyId
                           && DefaultFirstCurrName == _fromCurrency.IsoName
                           && DefaultSecondCurrId == _toCurrency.CurrencyId
                           && DefaultSecondCurrName == _toCurrency.IsoName));
        }

        public int CurrencyPairId { get; private set; }

        public string PairName
        {
            get => ToString();
            private set
            {
                if (IsDefault())
                {
                    return;
                }

                _name = value;
            }
        }

        public Currency FromCurrency
        {
            get => _fromCurrency;
            private set
            {
                if (IsDefault())
                {
                    return;
                }

                _fromCurrency = value ?? throw new ArgumentNullException("A CurrencyPair requires both currencies.");
            }
        }

        public Currency ToCurrency
        {
            get => _toCurrency;
            private set 
            {
                if (IsDefault())
                {
                    return;
                }

                _toCurrency = value ?? throw new ArgumentNullException("A CurrencyPair requires both currencies.");
            }
        }

        public CurrencyPair(int pairId, Currency fromCurr, Currency toCurr)
        {
            CurrencyPairId = pairId;
            FromCurrency = fromCurr;
            ToCurrency = toCurr;
            SetHashCodeStringRepresentation();
        }

        private void SetHashCodeStringRepresentation()
        {
            if (!String.IsNullOrWhiteSpace(_hashCodeStringRepresentation))
            {
                return;
            }

            if (FromCurrency.CurrencyId <= ToCurrency.CurrencyId)
            {
                _hashCodeStringRepresentation = FromCurrency.IsoName + ToCurrency.IsoName;
            }
            else
            {
                _hashCodeStringRepresentation = ToCurrency.IsoName + FromCurrency.IsoName;
            }
        }

        private void UpdateStringRepresentation()
        {
            if (FromCurrency == null || ToCurrency == null)
            {
                return;
            }

            _name = $"{FromCurrency.IsoName}/{ToCurrency.IsoName}";
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            CurrencyPair currencyPair = obj as CurrencyPair;

            if (null == (object)currencyPair)
            {
                return false;
            }

            return (currencyPair.FromCurrency.Equals(FromCurrency) && currencyPair.ToCurrency.Equals(ToCurrency))
                   || (currencyPair.FromCurrency.Equals(ToCurrency) && currencyPair.ToCurrency.Equals(FromCurrency));
        }

        public static bool operator ==(CurrencyPair lhs, CurrencyPair rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (null == (object)lhs)
            {
                return null == (object)rhs;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(CurrencyPair lhs, CurrencyPair rhs)
        {
            if (null == (object)lhs)
            {
                return null != (object)rhs;
            }

            return !lhs.Equals(rhs);
        }

        public bool Equals(int currency1Id, int currency2Id)
        {
            return (FromCurrency.CurrencyId == currency1Id && ToCurrency.CurrencyId == currency2Id)
                || (FromCurrency.CurrencyId == currency2Id && ToCurrency.CurrencyId == currency1Id);
        }

        public bool Equals(string currency1IsoName, string currency2IsoName)
        {
            return (FromCurrency.IsoName == currency1IsoName && ToCurrency.IsoName == currency2IsoName)
                || (FromCurrency.IsoName == currency2IsoName && ToCurrency.IsoName == currency1IsoName);
        }

        public override int GetHashCode()
        {
            return _hashCodeStringRepresentation.GetHashCode();
        }

        public override string ToString()
        {
            if (_name == null)
            {
                UpdateStringRepresentation();
            }

            return _name;
        }

        public bool IsInvertedPair(Currency fromCurrency, Currency toCurrency)
        {
            return fromCurrency == ToCurrency && toCurrency == FromCurrency;
        }

        public bool IsInvertedPair(int fromCurrencyId, int toCurrencyId)
        {
            return fromCurrencyId == ToCurrency.CurrencyId && toCurrencyId == FromCurrency.CurrencyId;
        }

        public bool IsInvertedPair(string fromCurrency, string toCurrency)
        {
            return fromCurrency == ToCurrency.IsoName && toCurrency == FromCurrency.IsoName;
        }
    }
}