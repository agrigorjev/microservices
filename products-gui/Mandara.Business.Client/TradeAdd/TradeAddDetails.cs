using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Extensions.Option;

namespace Mandara.Business.TradeAdd
{
    public delegate void PortfolioChangedEventHandler(object sender, EventArgs args);
    public delegate void ImpactChangeEventHandler(object sender, EventArgs args);
    public delegate void InstrumentChangedEventHandler(object sender, InstrumentChangedEventArgs args);

    public class InstrumentChangedEventArgs : EventArgs
    {
        public Instrument ChangedInstrument { get; set; }
    }

    public class TradeAddDetails : INotifyPropertyChanged
    {
        public TradeAddDetails()
        {
            FxExchangeRate = TradeAddDetails.DefaultFxExchangeRate;
            ForwardValueDate = SystemTime.Today().AddDays(1);
            IsSpot = true;
        }

        [Key]
        public int Key { get; set; }

        private string _broker;
        private Portfolio _portfolio1;
        private Portfolio _portfolio2;
        private string _exchange;
        private string _expiryExchange;
        private DateTime? _tradeDate;
        private DateTime? _timestampUtc;
        private DateTime? _transactTimeUtc;
        private SideControl _side;

        public event ImpactChangeEventHandler ImpactChange;
        public event PortfolioChangedEventHandler PortfolioChanged;

        public const decimal DefaultFxExchangeRate = 1.0M;

        [ForeignKey("name")]
        public Portfolio Portfolio1
        {
            get => _portfolio1;
            set
            {
                // TODO: This is horrible
                bool changed = _portfolio1 == null || value == null;

                if (!changed)
                {
                    changed = !_portfolio1.Equals(value);
                }

                _portfolio1 = value;

                if (!changed)
                {
                    return;
                }

                OnPropertyChanged(nameof(Portfolio1));
                FirePortfolioChanged();
            }
        }

        private void FirePortfolioChanged()
        {
            PortfolioChanged?.Invoke(this, EventArgs.Empty);
        }

        [ForeignKey("name")]
        public Portfolio Portfolio2
        {
            get => _portfolio2;

            set
            {
                if (Equals(value, _portfolio2))
                {
                    return;
                }

                _portfolio2 = value;
                OnPropertyChanged(nameof(Portfolio2));
            }
        }

        [Column]
        public string Broker
        {
            get => _broker;
            set
            {
                if (_broker == value)
                {
                    return;
                }

                _broker = value;
                FireImpactChange();
            }
        }

        [Column]
        public string Exchange
        {
            get => _exchange;
            set
            {
                if (_exchange == value)
                {
                    return;
                }

                _exchange = value;
                OnPropertyChanged(nameof(Exchange));
            }
        }

        [Column]
        public string ExpiryExchange
        {
            get => _expiryExchange;
            set
            {
                if (_expiryExchange == value)
                {
                    return;
                }

                _expiryExchange = value;
                OnPropertyChanged(nameof(ExpiryExchange));
            }
        }

        [Column]
        public bool IsInternalExchange => Exchange.Equals("internal", StringComparison.InvariantCultureIgnoreCase);

        [Column]
        public TradeTypeControl TradeType { get; set; }

        [Column]
        public SideControl Side
        {
            get => _side;
            set
            {
                if (value == _side)
                {
                    return;
                }

                _side = value;
                OnPropertyChanged(nameof(Side));
            }
        }

        [Column]
        public StripTypeControl StripTypeControl { get; set; }

        [Column]
        public StripDetail StripDetail1 { get; set; }

        [Column]
        public StripDetail StripDetail2 { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedByUserName { get; set; }

        public TradeAddPrerequisitesResponseMessage DataSources { get; set; }

        [ForeignKey("name")]
        public Product Product { get; set; }

        [Column]
        public int OfficialProductId { get; set; }

        public bool IsProductFromProductTool => Product != null;

        public List<int> TradeCaptureIds { get; set; }

        [Column]
        public int? GroupId { get; set; }

        [Column]
        public string EditCancelReason { get; set; }

        public bool? IsTasChecked { get; set; }
        public bool? IsMopsChecked { get; set; }
        public bool? IsMmChecked { get; set; }
        public bool? IsMocChecked { get; set; }

        public bool IsMasterToolMode { get; set; }

        [Column]
        public DateTime? TradeDate
        {
            get => _tradeDate;
            set
            {
                if (value?.Equals(_tradeDate) ?? !_tradeDate.HasValue)
                {
                    return;
                }

                _tradeDate = value;
                OnPropertyChanged(nameof(TradeDate));
            }
        }

        public void SetTradeDate(TryGetResult<DateTime> tradeDate)
        {
            TradeDate = tradeDate.HasValue ? tradeDate.Value : default(DateTime?);
        }

        [Column]
        public DateTime? Timestamp
        {
            get => _timestampUtc?.ToLocalTime();
        }

        [Column]
        public DateTime? TimestampUtc
        {
            get => _timestampUtc;
            set
            {
                if (value?.Equals(_timestampUtc) ?? !_timestampUtc.HasValue)
                {
                    return;
                }

                _timestampUtc = value?.ToUniversalTime();
                OnPropertyChanged(nameof(TimestampUtc));
            }
        }

        [Column]
        public DateTime? TransactTime
        {
            get => _transactTimeUtc?.ToLocalTime();
        }

        [Column]
        public DateTime? TransactTimeUtc
        {
            get => _transactTimeUtc;
            set
            {
                if (value?.Equals(_transactTimeUtc) ?? !_transactTimeUtc.HasValue)
                {
                    return;
                }

                _transactTimeUtc = value?.ToUniversalTime();
                OnPropertyChanged(nameof(TransactTimeUtc));
            }
        }

        public void SetTransactTime(TryGetResult<DateTime> localTransactTime)
        {
            TransactTimeUtc = localTransactTime.HasValue ? localTransactTime.Value.ToUniversalTime() : default(DateTime?);
        }

        public void FireImpactChange()
        {
            if (!ValidateTradeDetailsForImpactCalculations())
            {
                return;
            }

            ImpactChange?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidateTradeDetailsForImpactCalculations()
        {
            bool isValid = !String.IsNullOrEmpty(Exchange)
                           && StripDetail1?.Instrument != null
                           && StripDetail1.Volume > 0
                           && StripDetail1.Unit != null;

            switch (StripTypeControl)
            {
                case StripTypeControl.DailySwap:
                case StripTypeControl.DailyDiff:
                {
                    return isValid
                             && StripDetail1?.Strip != null
                             && StripDetail1.Strip.StartDate <= StripDetail1.Strip.EndDate;
                }

                case StripTypeControl.Spread:
                {
                    return isValid
                             && StripDetail1?.Strip != null
                             && StripDetail2?.Strip != null
                             && StripDetail1.Strip.StringValue != StripDetail2.Strip.StringValue;
                }
            }

            return isValid;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool WouldGenerateTwoTrades()
        {
            return null != StripDetail2
                   && (HasExactlyOneQOrCalStrip()
                       || SpreadHasABalmoStrip()
                       || StripTypeControl == StripTypeControl.FutureVsSwap);
        }

        private bool HasExactlyOneQOrCalStrip()
        {
            return IsStripAQOrCal(StripDetail1.Strip) ^ IsStripAQOrCal(StripDetail2.Strip);
        }

        private bool IsStripAQOrCal(Strip strip)
        {
            return strip.StringValue.Contains("Q") || strip.StringValue.Contains("Cal");
        }

        public bool SpreadHasABalmoStrip()
        {
            return StripDetail1.Strip.IsBalmoStrip || StripDetail2.Strip.IsBalmoStrip;
        }

        public bool IsSpreadWithOneQOrCalStrip()
        {
            return null != StripDetail2 && HasExactlyOneQOrCalStrip();
        }

        public DateTime ForwardValueDate { get; set; }

        public decimal SpecifiedAmount { get; set; }

        public decimal AgainstAmount
        {
            get
            {
                if (FxExchangeRate == 0M || FxSelectedInstrument == null)
                {
                    return SpecifiedAmount;
                }

                if (FxSelectedInstrument.FxSpecifiedCurrency == FxSelectedInstrument.Currency)
                {
                    return SpecifiedAmount / FxExchangeRate;
                }

                return SpecifiedAmount * FxExchangeRate;
            }
        }

        public decimal FxExchangeRate { get; set; }

        public bool IsSpot { get; set; }

		public Instrument FxSelectedInstrument { get; set; }


        public override string ToString()
        {
            return
                $"{nameof(Key)}: {Key}, {nameof(Portfolio1)}: {Portfolio1}, {nameof(Portfolio2)}: {Portfolio2}, {nameof(Broker)}: {Broker}, {nameof(Exchange)}: {Exchange}, {nameof(ExpiryExchange)}: {ExpiryExchange}, {nameof(TradeType)}: {TradeType}, {nameof(Side)}: {Side}, {nameof(StripTypeControl)}: {StripTypeControl}, {nameof(StripDetail1)}: {StripDetail1}, {nameof(StripDetail2)}: {StripDetail2}, {nameof(CreatedAtUtc)}: {CreatedAtUtc}, {nameof(CreatedByUserName)}: {CreatedByUserName}, {nameof(Product)}: {Product}, {nameof(OfficialProductId)}: {OfficialProductId}, {nameof(EditCancelReason)}: {EditCancelReason}, {nameof(TradeDate)}: {TradeDate}, {nameof(TimestampUtc)}: {TimestampUtc}, {nameof(TransactTimeUtc)}: {TransactTimeUtc}, {nameof(SpecifiedAmount)}: {SpecifiedAmount}, {nameof(AgainstAmount)}: {AgainstAmount}";
        }
	}
}