using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Trades;
using Mandara.Entities.TypeMapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Mandara.Date;

namespace Mandara.Business.Client.Managers
{
    public class LiveDataManager
    {
        public static readonly string CancelledStatusVariants = ConfigurationManager.AppSettings["TradeCancelledStatus"]
                                                                ?? "Cancelled|Canceled";
        public static readonly string ExchangesConvertTimeVariants =
            ConfigurationManager.AppSettings["Exchanges_ConvertTime"] ?? "CPC|Clearport";
        public static readonly string CustomMonthlyStripName = "Custom Monthly";
        public static readonly string DailySwapStripName = "Custom CFD";
        public static readonly string DailyDiffStripName = "Custom CFD (Month)";
        public static readonly string BalmoStripName = "Bal Month";
        public const string LAST_TRADE_ID_SETTING_NAME = "TradeAssignmentService_LastTradeId";

        public static readonly string ExchangesVariants = ConfigurationManager.AppSettings["Exchanges"]
                                                          ?? "ICE|Clearport|SGX|Internal";
        public static readonly string TasTradesVariants = ConfigurationManager.AppSettings["ExcludeTAS"]
                                                          ?? "TAS|Settlement";
        private static readonly string DayFirst = Formats.DayFirstDateFormat('/');
        public static readonly DateTime? MaxTradeDate =
            StripHelper.ParseDate(ConfigurationManager.AppSettings["MaxTrade_Date"], DayFirst);
        public static readonly DateTime? CutoffDate =
            StripHelper.ParseDate(ConfigurationManager.AppSettings["CutOff_Date"], DayFirst);

        public static decimal GetDecimalNumOfLots(
            int? numOfLots,
            int? tradeType,
            decimal? quantity,
            string stripName,
            DateTime? tradeStartDate,
            DateTime? tradeEndDate)
        {
            if (!numOfLots.HasValue || tradeType.HasValue)
            {
                return Math.Abs(
                    quantity.Value * GetNumberOfMonthsFromStripName(stripName, tradeStartDate, tradeEndDate));
            }

            return Math.Abs(numOfLots.Value);
        }

        public static decimal GetDecimalNumOfLots(TradeCapture trade)
        {
            if (!trade.NumOfLots.HasValue || trade.TradeType.HasValue)
            {
                return
                    Math.Abs(
                        trade.Quantity.Value
                        * GetNumberOfMonthsFromStripName(
                            trade.SecurityDefinition.StripName,
                            trade.TradeStartDate,
                            trade.TradeEndDate));
            }

            return Math.Abs(trade.NumOfLots.Value);
        }

        public static int GetNumberOfMonthsFromStripName(
            string stripName,
            object tradeStartDate = null,
            object tradeEndDate = null)
        {
            Tuple<DateTime, ProductDateType> liveTradeDate;

            try
            {
                liveTradeDate = StripHelper.ParseStripDate(stripName, SystemTime.Now(), SystemTime.Now());
            }
            catch
            {
                return 1;
            }

            switch (liveTradeDate.Item2)
            {
                case ProductDateType.Day:
                case ProductDateType.MonthYear:
                return 1;

                case ProductDateType.Quarter:
                return 3;

                case ProductDateType.Year:
                return 12;

                case ProductDateType.Custom:
                DateTime? startDate = StripHelper.ParseDate(tradeStartDate, Formats.SortableShortDate);
                DateTime? endDate = StripHelper.ParseDate(tradeEndDate, Formats.SortableShortDate);

                if (startDate.HasValue && endDate.HasValue)
                {
                    return Math.Abs(endDate.Value.MonthsSince(startDate.Value) + 1);
                }

                return 1;
            }

            return 1;
        }

        public static LiveTradeView ConvertFxTradeToTradeView(FxTrade fxTrade)
        {
            LiveTradeView tradeView = TradeToLiveTradeView.Convert<LiveTradeView>(fxTrade.TradeCapture);

            tradeView.FxTradeId = fxTrade.FxTradeId;
            tradeView.ProductType = fxTrade.ProductType;
            tradeView.SpecifiedAmount = fxTrade.SpecifiedAmount;
            tradeView.AgainstAmount = fxTrade.AgainstAmount;
            tradeView.Rate = fxTrade.Rate;
            tradeView.SpotRate = fxTrade.SpotRate;
            tradeView.Tenor = fxTrade.Tenor;
            tradeView.LinkType = fxTrade.LinkType;
            tradeView.ValueDate = fxTrade.ValueDate;
            tradeView.AgainstCurrency = fxTrade.AgainstCurrency.IsoName;
            tradeView.SpecifiedCurrency = fxTrade.SpecifiedCurrency.IsoName;
            tradeView.LinkTradeReportId = fxTrade.LinkTradeReportId;

            return tradeView;
        }

        public static bool IsTradeCancelled(TradeCapture trade)
        {
            if (trade == null)
            {
                return false;
            }

            return IsTradeCancelled(trade.OrdStatus);
        }

        public static bool IsTradeCancelled(string ordStatus)
        {
            List<string> list = CancelledStatusVariants.Split('|').ToList();

            return list.Contains(ordStatus);
        }

        public static List<LiveTradeView> ConvertFxTradeToTradeView(List<FxTrade> fxTrades)
        {
            return fxTrades.Select(ConvertFxTradeToTradeView).ToList();
        }
    }
}