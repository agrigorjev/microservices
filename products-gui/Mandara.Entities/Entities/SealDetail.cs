using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading;
using Mandara.Date;

namespace Mandara.Entities
{
    [Table("source_data_seals_details")]
    public partial class SealDetail
    {
        [Column("seals_detail_id")]
        [Key]
        public int seals_detail_id { get; set; }

        [Column("source_data_id")]
        public int SourceDataId { get; set; }

        [Column("date_type")]
        public short? DateType { get; set; }

        [Column("exchange_Id")]
        [StringLength(50)]
        public string ExchangeId { get; set; }

        [Column("volume")]
        [StringLength(50)]
        public string Volume { get; set; }

        [Column("market_product")]
        [StringLength(50)]
        public string Market_Product { get; set; }

        [Column("BuySell")]
        [StringLength(50)]
        public string BuySell { get; set; }

        [Column("market_expry")]
        [StringLength(50)]
        public string Market_Expiry { get; set; }

        [Column("price")]
        [StringLength(50)]
        public string Price { get; set; }

        [Column("exchange_trade_number")]
        [StringLength(50)]
        public string ExchangeTradeNumber { get; set; }

        [Column("exchange_order_number")]
        [StringLength(50)]
        public string ExchangeOrderNumber { get; set; }

        [Column("BusinessStateName")]
        [StringLength(50)]
        public string BusinessStateName { get; set; }

        [Column("TradeId")]
        [StringLength(50)]
        public string TradeId { get; set; }

        [Column("OptionType")]
        [StringLength(50)]
        public string OptionType { get; set; }

        [Column("Strike")]
        [StringLength(50)]
        public string Strike { get; set; }

        [Column("Expr1")]
        [StringLength(50)]
        public string Expr1 { get; set; }

        [Column("Reference1")]
        [StringLength(50)]
        public string Reference1 { get; set; }

        [Column("ToMemberId")]
        [StringLength(50)]
        public string ToMemberId { get; set; }

        [Column("ToMember")]
        [StringLength(50)]
        public string ToMember { get; set; }

        [Column("Reference2")]
        [StringLength(50)]
        public string Reference2 { get; set; }

        [Column("Reference3")]
        [StringLength(50)]
        public string Reference3 { get; set; }

        [Column("GiveupRef1")]
        [StringLength(50)]
        public string GiveupRef1 { get; set; }

        [Column("GiveupRef2")]
        [StringLength(50)]
        public string GiveupRef2 { get; set; }

        [Column("GiveupRef3")]
        [StringLength(50)]
        public string GiveupRef3 { get; set; }

        [Column("Trader")]
        [StringLength(50)]
        public string Trader { get; set; }

        [Column("AllocationId")]
        [StringLength(50)]
        public string AllocationId { get; set; }

        [Column("ExchangeMember")]
        [StringLength(50)]
        public string ExchangeMember { get; set; }

        [Column("TradingDay")]
        [StringLength(50)]
        public string TradingDay { get; set; }

        [Column("ClearingDay")]
        [StringLength(50)]
        public string ClearingDay { get; set; }

        [Column("CounterPartyMember")]
        [StringLength(50)]
        public string CounterPartyMember { get; set; }

        [Column("OpenClose")]
        [StringLength(50)]
        public string OpenClose { get; set; }

        [Column("PositionAccount")]
        [StringLength(50)]
        public string PositionAccount { get; set; }

        [Column("BOAccount")]
        [StringLength(50)]
        public string BOAccount { get; set; }

        [Column("BOReference")]
        [StringLength(50)]
        public string BOReference { get; set; }

        [Column("OriginTransferType")]
        [StringLength(50)]
        public string OriginTransferType { get; set; }

        [Column("OriginFromMember")]
        [StringLength(50)]
        public string OriginFromMember { get; set; }

        [Column("TradingTradeType")]
        [StringLength(50)]
        public string TradingTradeType { get; set; }

        [Column("TradePriceType")]
        [StringLength(50)]
        public string TradePriceType { get; set; }

        [Column("Venue")]
        [StringLength(50)]
        public string Venue { get; set; }

        [Column("ComboType")]
        [StringLength(50)]
        public string ComboType { get; set; }

        [Column("OriginalClearingDay")]
        [StringLength(50)]
        public string OriginalClearingDay { get; set; }

        [Column("Note")]
        [StringLength(50)]
        public string Note { get; set; }

        [Column("TradeGroupId")]
        [StringLength(50)]
        public string TradeGroupId { get; set; }

        [Column("ClientId")]
        [StringLength(50)]
        public string ClientId { get; set; }

        [Column("ClientName")]
        [StringLength(50)]
        public string ClientName { get; set; }

        [ForeignKey("SourceDataId")]
        public virtual SourceData SourceData
        {
            get { return _sourceData; }
            set
            {
                _sourceData = value;
                SourceDataId = _sourceData != null ? _sourceData.SourceDataId : 0;
            }
        }

        #region Direct conversion of db values
        private DateTime _marketExpiry;
        private SourceData _sourceData;

        [NotMapped]
        public DateTime MarketExpiry
        {
            get
            {
                //If not set explicitly try to get from Market_Expiry field
                if (_marketExpiry == DateTime.MinValue)
                {
                    try
                    {
                        DateTime r;

                        if (!DateTime.TryParseExact(Market_Expiry, Formats.SortableShortDate, CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None, out r))
                        {
                            if (Market_Expiry.Length == 8 && Market_Expiry.EndsWith("00"))
                            {
                                string checkDate = Market_Expiry.Substring(0, 7) + "1";
                                if (DateTime.TryParseExact(checkDate, Formats.SortableShortDate, CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None, out r))
                                {
                                    r = r.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    r = DateTime.Parse(Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                                }
                            }
                            else
                            {
                                r = DateTime.Parse(Market_Expiry, CultureInfo.GetCultureInfo("en-GB"));
                            }
                        }
                        return r;
                    }
                    catch
                    {
                        throw new FormatException(string.Format(
                            "Market_Expiry string was not recognized as a valid DateTime [{0}].", Market_Expiry));
                    }
                }
                else
                {
                    return _marketExpiry;
                }
            }
            //set explicitly for seals balmo processing from ICE exchange
            set
            {
                _marketExpiry = value;
            }
        }

        [NotMapped]
        public decimal ConvertedPrice
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(Price);
                }
                catch
                {
                    throw new FormatException(string.Format("Following string couldn't be parsed as Price [{0}].",
                                                            Price));
                }
            }
        }

        [NotMapped]
        public decimal Quantity
        {
            get
            {

                decimal volume = 0;
                if (decimal.TryParse(Volume, out volume))
                {
                    volume = BuySell.ToLower().StartsWith("b") ? volume : (volume * -1);
                    return volume;
                }
                else
                {
                    throw new FormatException(string.Format("Following string couldn't be parsed as Volume [{0}].",
                                                        Volume));
                }
            }
        }
        #endregion
        //Mapped externally


        [NotMapped]
        public Product Product { get; set; }
        /// <summary>
        /// Check if current Seals Detail from ICE Exchange
        /// </summary>

        [NotMapped]
        public bool IsICE
        {
            get
            {
                if (!string.IsNullOrEmpty(ExchangeId))
                {
                    string iceExchanges = System.Configuration.ConfigurationManager.AppSettings["IceSealsExchanges"] ?? "ICE|IOTC";
                    return iceExchanges.Split('|').Contains(ExchangeId);
                }
                else
                {
                    return false;
                }
            }
        }

    }
    /// <summary>
    /// Static class extending Seal details for class coversion
    /// </summary>
    public static class SealDetailExtra
    {
        /// <summary>
        /// Convert Seal detail to source detail to calculate positions
        /// </summary>
        /// <param name="src">Self link</param>
        /// <returns>SourceDetail representation of seal details data</returns>
        public static SourceDetail ToSourceDetail(this SealDetail src)
        {
            //Backup current culture
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            //Change culture to neutral for data conversion
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            SourceDetail tRest = new SourceDetail();
            try
            {
                tRest = new SourceDetail()
                {
                    #region setup common fields
                    DateTypeDb = (short)src.DateType.GetValueOrDefault(),
                    Exchange_Code = src.ExchangeTradeNumber,
                    OrderNumber = src.ExchangeOrderNumber,
                    InstrumentDescription = src.Product.Name,

                    Product = src.Product,
                    ProductDate = src.MarketExpiry,
                    Quantity = src.Quantity,
                    SourceData = src.SourceData,
                    SourceDetailId = src.seals_detail_id,
                    TradePrice = src.ConvertedPrice,
                    #endregion
                    ////Seals Extension according IRM-237
                    #region setup specific fields
                    BusinessStateName = src.BusinessStateName,
                    TradeId = src.TradeId,
                    ExchangeId = src.ExchangeId,
                    Volume = src.Volume,
                    Market_Expiry = src.Market_Expiry,
                    Market_Product = src.Market_Product,
                    BuySell = src.BuySell,
                    OptionType = src.OptionType,
                    Strike = src.Strike,
                    Price = src.Price,
                    Expr1 = src.Expr1,
                    Reference1 = src.Reference1,
                    ToMemberId = src.ToMemberId,
                    ToMember = src.ToMember,
                    ExchangeOrderNumber = src.ExchangeOrderNumber,
                    ExchangeTradeNumber = src.ExchangeTradeNumber,
                    Reference2 = src.Reference2,
                    Reference3 = src.Reference3,
                    GiveupRef1 = src.GiveupRef1,
                    GiveupRef2 = src.GiveupRef2,
                    GiveupRef3 = src.GiveupRef3,
                    Trader = src.Trader,
                    AllocationId = src.AllocationId,
                    ExchangeMember = src.ExchangeMember,
                    TradingDay = src.TradingDay,
                    ClearingDay = src.ClearingDay,
                    CounterPartyMember = src.CounterPartyMember,
                    OpenClose = src.OpenClose,
                    PositionAccount = src.PositionAccount,
                    BOAccount = src.BOAccount,
                    BOReference = src.BOReference,
                    OriginTransferType = src.OriginTransferType,
                    OriginFromMember = src.OriginFromMember,
                    TradingTradeType = src.TradingTradeType,
                    TradePriceType = src.TradePriceType,
                    Venue = src.Venue,
                    ComboType = src.ComboType,
                    OriginalClearingDay = src.OriginalClearingDay,
                    Note = src.Note,
                    TradeGroupId = src.TradeGroupId,
                    ClientId = src.ClientId,
                    ClientName = src.ClientName
                    #endregion
                };

            }
            finally
            {
                //Set original culture back
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
            return tRest;

        }
        /// <summary>
        /// Return only db dcontent. No datatype conversion
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static SourceDetail SafeToSourceDetail(this SealDetail src)
        {
            SourceDetail tRest = new SourceDetail()
            {
                //AccountNumber = null,
                SourceData = src.SourceData,
                SourceDetailId = src.seals_detail_id,
                ////Seals Extension according IRM-237
                Exchange_Code = src.Market_Product,
                BusinessStateName = src.BusinessStateName,
                TradeId = src.TradeId,
                ExchangeId = src.ExchangeId,
                Volume = src.Volume,
                Market_Expiry = src.Market_Expiry,
                Market_Product = src.Market_Product,
                BuySell = src.BuySell,
                OptionType = src.OptionType,
                Strike = src.Strike,
                Price = src.Price,
                Expr1 = src.Expr1,
                Reference1 = src.Reference1,
                ToMemberId = src.ToMemberId,
                ToMember = src.ToMember,
                ExchangeOrderNumber = src.ExchangeOrderNumber,
                ExchangeTradeNumber = src.ExchangeTradeNumber,
                Reference2 = src.Reference2,
                Reference3 = src.Reference3,
                GiveupRef1 = src.GiveupRef1,
                GiveupRef2 = src.GiveupRef2,
                GiveupRef3 = src.GiveupRef3,
                Trader = src.Trader,
                AllocationId = src.AllocationId,
                ExchangeMember = src.ExchangeMember,
                TradingDay = src.TradingDay,
                ClearingDay = src.ClearingDay,
                CounterPartyMember = src.CounterPartyMember,
                OpenClose = src.OpenClose,
                PositionAccount = src.PositionAccount,
                BOAccount = src.BOAccount,
                BOReference = src.BOReference,
                OriginTransferType = src.OriginTransferType,
                OriginFromMember = src.OriginFromMember,
                TradingTradeType = src.TradingTradeType,
                TradePriceType = src.TradePriceType,
                Venue = src.Venue,
                ComboType = src.ComboType,
                OriginalClearingDay = src.OriginalClearingDay,
                Note = src.Note,
                TradeGroupId = src.TradeGroupId,
                ClientId = src.ClientId,
                ClientName = src.ClientName,


            };
            return tRest;

        }

    }
}
