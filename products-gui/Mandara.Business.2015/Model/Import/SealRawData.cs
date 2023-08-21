using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Mandara.Business;
using Mandara.Date;
using Mandara.Entities.ErrorReporting;
using Mandara.Entities.Exceptions;

namespace Mandara.Import
{
    /// <summary>
    /// Support class for transformation of seals csv values to SealsDetail object.Every meaningful field attributed with csv header text. [RawData] attribute
    /// </summary>
    public class SealRawData
    {
        [RawData(FieldName = "BusinessStateName")]
        public string Raw_BusinessStateName { get; set; }
        [RawData(FieldName = "TradeId")]
        public string Raw_TradeId { get; set; }
        [RawData(FieldName = "ExchangeId")]
        public string Raw_ExchangeId { get; set; }
        [RawData(FieldName = "Volume")]
        public string Raw_Volume { get; set; }
        [RawData(FieldName = "Market_Expiry")]
        public string Raw_Market_Expiry
        {
            get;
            set;

        }
        [RawData(FieldName = "Market_Product")]
        public string Raw_Market_Product { get; set; }
        [RawData(FieldName = "BuySell")]
        public string Raw_BuySell { get; set; }
        [RawData(FieldName = "OptionType")]
        public string Raw_OptionType { get; set; }
        [RawData(FieldName = "Strike")]
        public string Raw_Strike { get; set; }
        [RawData(FieldName = "Price")]
        public string Raw_Price { get; set; }
        [RawData(FieldName = "Expr1")]
        public string Raw_Expr1 { get; set; }
        [RawData(FieldName = "Reference1")]
        public string Raw_Reference1 { get; set; }
        [RawData(FieldName = "ToMemberId")]
        public string Raw_ToMemberId { get; set; }
        [RawData(FieldName = "ToMember")]
        public string Raw_ToMember { get; set; }
        [RawData(FieldName = "ExchangeOrderNumber")]
        public string Raw_ExchangeOrderNumber { get; set; }
        [RawData(FieldName = "ExchangeTradeNumber")]
        public string Raw_ExchangeTradeNumber { get; set; }
        [RawData(FieldName = "Reference2")]
        public string Raw_Reference2 { get; set; }
        [RawData(FieldName = "Reference3")]
        public string Raw_Reference3 { get; set; }
        [RawData(FieldName = "GiveupRef1")]
        public string Raw_GiveupRef1 { get; set; }
        [RawData(FieldName = "GiveupRef2")]
        public string Raw_GiveupRef2 { get; set; }
        [RawData(FieldName = "GiveupRef3")]
        public string Raw_GiveupRef3 { get; set; }
        [RawData(FieldName = "Trader")]
        public string Raw_Trader { get; set; }
        [RawData(FieldName = "AllocationId")]
        public string Raw_AllocationId { get; set; }
        [RawData(FieldName = "ExchangeMember")]
        public string Raw_ExchangeMember { get; set; }
        [RawData(FieldName = "TradingDay")]
        public string Raw_TradingDay { get; set; }
        [RawData(FieldName = "ClearingDay")]
        public string Raw_ClearingDay { get; set; }
        [RawData(FieldName = "CounterPartyMember")]
        public string Raw_CounterPartyMember { get; set; }
        [RawData(FieldName = "OpenClose")]
        public string Raw_OpenClose { get; set; }
        [RawData(FieldName = "PositionAccount")]
        public string Raw_PositionAccount { get; set; }
        [RawData(FieldName = "BOAccount")]
        public string Raw_BOAccount { get; set; }
        [RawData(FieldName = "BOReference")]
        public string Raw_BOReference { get; set; }
        [RawData(FieldName = "OriginTransferType")]
        public string Raw_OriginTransferType { get; set; }
        [RawData(FieldName = "OriginFromMember")]
        public string Raw_OriginFromMember { get; set; }
        [RawData(FieldName = "TradingTradeType")]
        public string Raw_TradingTradeType { get; set; }
        [RawData(FieldName = "TradePriceType")]
        public string Raw_TradePriceType { get; set; }
        [RawData(FieldName = "Venue")]
        public string Raw_Venue { get; set; }
        [RawData(FieldName = "ComboType")]
        public string Raw_ComboType { get; set; }
        [RawData(FieldName = "OriginalClearingDay")]
        public string Raw_OriginalClearingDay { get; set; }
        [RawData(FieldName = "Note")]
        public string Raw_Note { get; set; }
        [RawData(FieldName = "TradeGroupId")]
        public string Raw_TradeGroupId { get; set; }
        [RawData(FieldName = "ClientId")]
        public string Raw_ClientId { get; set; }
        [RawData(FieldName = "ClientName")]
        public string Raw_ClientName { get; set; }
        /// <summary>
        ///Convert Market_Expiry to DateTime to determine if balmo.
        /// </summary>
        public DateTime? Expiry
        {
            get
            {

                try
                {
                    DateTime r;
                    if (!DateTime.TryParseExact(
                        Raw_Market_Expiry,
                        Formats.SortableShortDate,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out r))
                    {
                        if (Raw_Market_Expiry.Length == 8 && Raw_Market_Expiry.EndsWith("00"))
                        {
                            string checkDate = Raw_Market_Expiry.Substring(0, 7) + "1";
                            if (DateTime.TryParseExact(
                                checkDate,
                                Formats.SortableShortDate,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out r))
                            {
                                r = r.AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                r = DateTime.Parse(Raw_Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                            }
                        }
                        else
                        {
                            r = DateTime.Parse(Raw_Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                        }
                    }
                    return r;

                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Check csv data validity for several fields (Market_Expiry id DAteTime,
        /// Price is decimal, Volume is decimal).
        /// </summary>
        /// <returns></returns>
        public List<DataImportException> CheckRawData()
        {
            List<DataImportException> exceptions = new List<DataImportException>();

            try
            {
                DateTime r;

                if (!DateTime.TryParseExact(
                    Raw_Market_Expiry,
                    Formats.SortableShortDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out r))
                {
                    if (Raw_Market_Expiry.Length == 8 && Raw_Market_Expiry.EndsWith("00"))
                    {
                        string checkDate = Raw_Market_Expiry.Substring(0, 7) + "1";
                        if (DateTime.TryParseExact(
                            checkDate,
                            Formats.SortableShortDate,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out r))
                        {
                            r = r.AddMonths(1).AddDays(-1);
                        }
                        else
                        {
                            r = DateTime.Parse(Raw_Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                        }
                    }
                    else
                    {
                        r = DateTime.Parse(Raw_Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                    }
                }

            }
            catch
            {
                exceptions.Add(new DataImportException(string.Format(
                    "Market_Expiry string was not recognized as a valid DateTime [{0}].", Raw_Market_Expiry), 0));
            }

            decimal tst;
            if (!decimal.TryParse(Raw_Price, out tst))
            {
                if (Raw_Price != null)
                {
                    string oldPrice = Raw_Price;
                    decimal? price = SealsPriceParser.ParsePrice(Raw_Price);

                    if (price.HasValue)
                        Raw_Price = price.ToString();

                    DataImportException dataImportException = new DataImportException(string.Format(
                        "Price string [{0}] was parsed as a [{1}] price.", oldPrice, Raw_Price), 0);
                    dataImportException.ErrorLevel = ErrorLevel.Low;

                    exceptions.Add(dataImportException);
                }
                else
                {
                    exceptions.Add(new DataImportException(string.Format(
                        "Price string was not recognized as a valid Price [{0}].", Raw_Price), 0));
                }
            }


            if (!decimal.TryParse(Raw_Volume, out tst))
            {
                exceptions.Add(new DataImportException(string.Format(
                        "Volume string was not recognized as a valid Volume [{0}].", Raw_Volume), 0));
            }

            return exceptions;
        }
    }
}
