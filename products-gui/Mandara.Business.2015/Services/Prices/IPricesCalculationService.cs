using Mandara.Business.Model;
using Mandara.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Services.Prices
{
    public class PriceCalcResult
    {
        private bool _livePriceSet = false;
        private LivePrice _live;

        public LivePrice Live
        {
            get
            {
                return _live;
            }
            set
            {
                if (!_livePriceSet && null != value)
                {
                    _livePriceSet = true;
                    _live = value;
                }
            }
        }

        private bool _tradePriceSet = false;
        private TradePrice _trade;
        public TradePrice Trade
        {
            get
            {
                return _trade;
            }
            set
            {
                if (!_tradePriceSet && null != value)
                {
                    _tradePriceSet = true;
                    _trade = value;
                }
            }
        }

        private int? _tradeId;

        public int TradeId
        {
            get
            {
                return _tradeId ?? -1;
            }

            set
            {
                if (!_tradeId.HasValue && value > 0)
                {
                    _tradeId = value;
                }
            }
        }

        private bool? _usedSettle;
        public bool UsedSettle
        {
            get
            {
                return _usedSettle.True();
            }
            set
            {
                if (!_usedSettle.HasValue)
                {
                    _usedSettle = value;
                }
            }
        }

        private Dictionary<int, decimal> _settles = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> Settles
        {
            get
            {
                return _settles;
            }
            set
            {
                if (!_settles.Any() && null != value && value.Any(offProdSettle => offProdSettle.Value != 0))
                {
                    _settles = value;
                }
            }
        }

        private decimal _partialPrice = Decimal.MinValue;
        public decimal PartialPrice
        {
            get
            {
                return _partialPrice;
            }
            set
            {
                if (Decimal.MinValue == _partialPrice)
                {
                    _partialPrice = value;
                }
            }
        }

        private decimal _partialFxPrice = Decimal.MinValue;
        public decimal PartialFxPrice
        {
            get
            {
                return _partialFxPrice;
            }
            set
            {
                if (Decimal.MinValue == _partialFxPrice)
                {
                    _partialFxPrice = value;
                }
            }
        }

        private PriceCalcResult()
        {
            _live = new LivePrice();
            _trade = new TradePrice();
        }

        public PriceCalcResult(LivePrice live, TradePrice trade)
        {
            Live = live;
            Trade = trade;
            UsedSettle = false;
        }

        public static PriceCalcResult Default => new PriceCalcResult();

        public bool IsDefault()
        {
            return !_livePriceSet
                   && !_tradePriceSet
                   && !_tradeId.HasValue
                   && !_usedSettle.HasValue
                   && Decimal.MinValue == _partialPrice
                   && Decimal.MinValue == _partialFxPrice;
        }

        public override string ToString()
        {
            if (IsDefault())
            {
                return "PriceCalcResult.Default";
            }

            StringBuilder priceCalc = new StringBuilder();

            priceCalc.AppendFormat(
                "Trade [{0}] has trade price [{1}] and live price [{2}]",
                TradeId,
                GetPrice(Trade.Leg1Price, Trade.Leg2Price, Trade.TradedPrice),
                GetPrice(Live.Leg1LivePrice, Live.Leg2LivePrice, Live.TradeLivePrice));
            priceCalc.Append(
                _usedSettle.True() ? $" with settle price(s) [{GetSettles(Settles)}]" : " with no settle price");
            priceCalc.Append(
                Decimal.MinValue == PartialPrice
                    ? " with no partial month price"
                    : $" with partial month price [{PartialPrice}]");
            priceCalc.Append(
                Decimal.MinValue == PartialFxPrice
                    ? " with no partial FX month price"
                    : $" with partial FX month price [{PartialFxPrice}]");
            return priceCalc.ToString();
        }

        private string GetPrice(Money? leg1Price, Money? leg2Price, Money? tradePrice)
        {
            return leg1Price.HasValue ? $"{leg1Price} / {leg2Price ?? Money.Default}" : $"{tradePrice}";
        }

        private string GetSettles(Dictionary<int, decimal> settles)
        {
            return String.Join(" | ", settles.Select(offProdSettle => $"({offProdSettle.Key}; {offProdSettle.Value})"));
        }
    }


    public interface IPricesCalculationService
    {
        void Initialise();
        bool TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePrices,
            out LivePrice livePrice,
            out TradePrice tradePrice);
        PriceCalcResult TryCalculateLivePrice(
            TradeCapture trade,
            DateTime riskDate,
            IPricesProvider livePrices);
    }


}