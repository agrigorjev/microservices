using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Mandara.Business.Client.Managers;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;

namespace Mandara.Business.TradeAdd
{
    public class Strip : INotifyPropertyChanged
    {
        private TradeAddDetails _tradeAddDetails;
        private DateTime _startDate;
        private DateTime? _endDate;

        public string StringValue { get; set; }
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                bool changed = _startDate != value;
                _startDate = value;

                if (changed)
                {
                    OnPropertyChanged("StartDate");
                    FireImpactChange();
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                bool changed = _endDate != value;
                _endDate = value;

                if (changed)
                {
                    OnPropertyChanged("EndDate");
                    FireImpactChange();
                }
            }
        }

        public bool IsBalmoStrip { get; set; }

        private void FireImpactChange()
        {
            if (_tradeAddDetails != null)
            {
                _tradeAddDetails.FireImpactChange();
            }
        }

        public override string ToString()
        {
            return StringValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetTradeAddDetails(TradeAddDetails tradeAddDetails)
        {
            _tradeAddDetails = tradeAddDetails;
        }

        public static Strip FromTradeCapture(TradeCapture tradeCapture, bool useFirstStrip)
        {
            return FromTradeCapture(tradeCapture, DefaultStripNameSelector(useFirstStrip));
        }

        public static Func<string[], string> DefaultStripNameSelector(bool useFirstStrip)
        {
            return (stripParts) => useFirstStrip ? stripParts[0] : stripParts[1];
        }

        public static Strip FromTradeCapture(TradeCapture tradeCapture, Func<string[], string> selectStripName)
        {
            SecurityDefinition secDef = tradeCapture.SecurityDefinition;

            return secDef.Product.Type.IsDaily()
                ? GetDailyProductStrip(tradeCapture, secDef.Product.Type)
                : GetNonDailyProductStrip(tradeCapture, secDef, selectStripName);
        }

        private static Strip GetDailyProductStrip(TradeCapture trade, ProductType tradedType)
        {
            Strip dailyStrip = new Strip
            {
                StringValue = ProductType.DailySwap == tradedType
                ? LiveDataManager.DailySwapStripName
                : LiveDataManager.DailyDiffStripName,
                StartDate = trade.TradeStartDate.Value,
                EndDate = trade.TradeEndDate.Value
            };
            return dailyStrip;
        }

        private static Strip GetNonDailyProductStrip(
            TradeCapture trade,
            SecurityDefinition security,
            Func<string[], string> selectStripName)
        {
            Strip strip = new Strip();
            string[] stripParts = security.StripName.Split('/');
            string stripName = selectStripName(stripParts);

            Tuple<DateTime, ProductDateType> liveTradeDate = StripHelper.ParseStripDate(
                stripName,
                trade.TradeStartDate ?? DateTime.MinValue,
                trade.TransactTime);

            strip.StringValue = stripName;
            strip.StartDate = liveTradeDate.Item1;

            if (liveTradeDate.Item2 == ProductDateType.Day)
            {
                strip.StringValue = "Balmo";
                strip.EndDate = trade.TradeEndDate;
                strip.IsBalmoStrip = true;
            }

            if (liveTradeDate.Item2 == ProductDateType.Custom)
            {
                strip.EndDate = trade.TradeEndDate;
            }

            return strip;
        }

        public static Strip FromTradeCapture(TradePieces tradeAndSecDef, Func<string[], string> selectStripName)
        {
            ProductType tradedType = tradeAndSecDef.Security.Product.Type;
            TradeCapture trade = tradeAndSecDef.Trade;
            SecurityDefinition security = tradeAndSecDef.Security.SecurityDef;

            return tradedType.IsDaily()
                ? GetDailyProductStrip(trade, tradedType)
                : GetNonDailyProductStrip(trade, security, selectStripName);
        }
    }
}