using System;
using System.Globalization;
using Mandara.Date;

namespace Mandara.Import
{
    public class SourceRawData
    {
        [RawData(FieldName = "Currency Symbol")]
        public string CurrencySymbol { get; set; }

        [RawData(FieldName = "Expiry Date")]
        public string RawExpiryDate { get; set; }

        [RawData(FieldName = "Multiplier")]
        public string RawMultiplier { get; set; }

        [RawData(FieldName = "customer name")]
        public string CustomerName { get; set; }

        [RawData(FieldName = "Process Date")]
        public string RawProcessDate { get; set; }

        [RawData(FieldName = "Instrument Description")]
        public string RawInstrumentDescription { get; set; }

        [RawData(FieldName = "Trade Price")]
        public string RawTradePrice { get; set; }

        [RawData(FieldName = "Quantity")]
        public string RawQuantity { get; set; }

        [RawData(FieldName = "Buy/Sell")]
        public string RawBuySell { get; set; }

        [RawData(FieldName = "Account Number")]
        public string AccountNumber { get; set; }

        [RawData(FieldName = "Exchange Code")]
        public string RawExchangeCode { get; set; }

        [RawData(FieldName = "Trade Date")]
        public string RawTradeDate { get; set; }

        [RawData(FieldName = "Trade Type")]
        public string RawTradeType { get; set; }

        [RawData(FieldName = "Clearing Fee")]
        public string RawClearingFee { get; set; }

        [RawData(FieldName = "Commission")]
        public string RawComissionFee { get; set; }

        [RawData(FieldName = "Exchange Fee")]
        public string RawExchangeFee { get; set; }

        [RawData(FieldName = "NFA Fee")]
        public string RawNfaFee { get; set; }

        [RawData(FieldName = "Futures Code")]
        public string RawFutureCode { get; set; }

        public Decimal Quantity
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(RawQuantity, new CultureInfo("en-US")) * (IsBuy ? 1 : -1);
                }
                catch
                {
                    throw new FormatException(
                        string.Format("Following string couldn't be parsed as Quantity [{0}].", RawQuantity));
                }
            }
        }

        public bool IsBuy
        {
            get { return RawBuySell.ToUpper() == "B"; }
        }

        public decimal TradePrice
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(RawTradePrice);
                }
                catch
                {
                    throw new FormatException(string.Format("Following string couldn't be parsed as Price [{0}].",
                                                            RawTradePrice));
                }
            }
        }

        public decimal? ClearingFee
        {
            get
            {
                if (string.IsNullOrEmpty(RawClearingFee))
                    return null;

                try
                {
                    return Convert.ToDecimal(RawClearingFee);
                }
                catch
                {
                    throw new FormatException(string.Format("The following string could not be parsed as Clearing Fee [{0}].",
                                                            RawClearingFee));
                }
            }
        }

        public decimal? ComissionFee
        {
            get
            {
                if (string.IsNullOrEmpty(RawComissionFee))
                    return null;

                try
                {
                    return Convert.ToDecimal(RawComissionFee);
                }
                catch
                {
                    throw new FormatException(string.Format("The following string could not be parsed as Comission Fee [{0}].",
                                                            RawComissionFee));
                }
            }
        }

        public decimal? ExchangeFee
        {
            get
            {
                if (string.IsNullOrEmpty(RawExchangeFee))
                    return null;

                try
                {
                    return Convert.ToDecimal(RawExchangeFee);
                }
                catch
                {
                    throw new FormatException(string.Format("The following string could not be parsed as Exchange Fee [{0}].",
                                                            RawExchangeFee));
                }
            }
        }

        public decimal? NfaFee
        {
            get
            {
                if (string.IsNullOrEmpty(RawNfaFee))
                    return null;

                try
                {
                    return Convert.ToDecimal(RawNfaFee);
                }
                catch
                {
                    throw new FormatException($"Invalid NFA Fee [{RawNfaFee}].");
                }
            }
        }

        public DateTime ProcessDate => DateParse.ParseDayFirst(RawProcessDate, "ProcessedDate");

        public DateTime TradeDate => DateParse.ParseDayFirst(RawTradeDate, "TradeDate");

        public DateTime ExpiryDate
        {
            get
            {
                try
                {
                    if (!DateTime.TryParseExact(
                        RawExpiryDate,
                        Formats.SortableShortDate,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime expiry))
                    {
                        expiry = DateTime.Parse(RawTradeDate, CultureInfo.GetCultureInfo("en-GB"));
                    }
                    
                    return expiry;
                }
                catch
                {
                    throw new FormatException(
                        $"ExpiryDate string not recognised as a valid DateTime [{RawExpiryDate}].");
                }
            }
        }

        public string ExchangeCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawExchangeCode))
                {
                    throw new FormatException("The Exchange Code is empty");
                }

                return Int32.TryParse(RawExchangeCode, out int exchangeCode)
                    ? exchangeCode.ToString()
                    : RawExchangeCode;
            }
        }
        public string FutureCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawFutureCode))
                {
                    throw new FormatException("The Futures Code could not be obtained empty string");
                }
                else
                    return RawFutureCode;
            }
        }
        public decimal Multiplier
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(RawMultiplier);
                }
                catch
                {
                    throw new FormatException(string.Format("Following string couldn't be parsed as Multiplier [{0}].",
                                                                 RawMultiplier));
                }
            }
        }
    }
}
