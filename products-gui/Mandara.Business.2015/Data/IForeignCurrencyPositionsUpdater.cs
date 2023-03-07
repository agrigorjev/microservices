using Mandara.Business.Services.Prices;
using Mandara.Entities;
using Mandara.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Data
{
    public class ForeignCurrencyPositionData
    {
        private List<ForeignCurrencyPosition> _positions;
        private List<Tuple<ForeignCurrencyPositionDetail, LivePnL>> _positionsDetails;
        private List<LivePnL> _pnlInputs;

        public IEnumerable<ForeignCurrencyPosition> Positions
        {
            get
            {
                return _positions;
            }
            private set
            {
                _positions = value.ToList();
            }
        }

        public IEnumerable<Tuple<ForeignCurrencyPositionDetail, LivePnL>> PositionsDetails
        {
            get
            {
                return _positionsDetails;
            }
            private set
            {
                _positionsDetails = value.ToList();
            }
        }

        public IEnumerable<LivePnL> PnLInputs
        {
            get
            {
                return _pnlInputs;
            }
            private set
            {
                _pnlInputs = value.ToList();
            }
        }

        public ForeignCurrencyPositionData(
            IEnumerable<ForeignCurrencyPosition> positions,
            IEnumerable<Tuple<ForeignCurrencyPositionDetail, LivePnL>> posDetails)
        {
            Positions = ConstructCollectionFromReceivedData(positions);
            PositionsDetails = ConstructCollectionFromReceivedData(posDetails);
        }

        private List<T> ConstructCollectionFromReceivedData<T>(IEnumerable<T> receivedColl)
        {
            return null == receivedColl ? new List<T>() : new List<T>(receivedColl);
        }
    }

    public class LivePnL
    {
        private Money pnl = Money.Default;

        public Money PnL
        {
            get
            {
                return pnl;
            }
            set
            {
                if (pnl.IsDefault() && !value.IsDefault())
                {
                    pnl = value;
                }
            }
        }

        private Money price = Money.Default;

        public Money Price
        {
            get
            {
                return price;
            }
            set
            {
                if (price.IsDefault() && !value.IsDefault())
                {
                    price = value;
                }
            }
        }

        private PriceCalcResult _priceCalc = PriceCalcResult.Default;
        public PriceCalcResult CalculatedPriceData
        {
            get
            {
                return _priceCalc;
            }
            set
            {
                if (_priceCalc.IsDefault() && null != value && !value.IsDefault())
                {
                    _priceCalc = value;
                }
            }
        }

        public static LivePnL Default => new LivePnL(PriceCalcResult.Default);


        public LivePnL(PriceCalcResult calculatedPriceData)
        {
            CalculatedPriceData = calculatedPriceData;
        }

        public bool IsDefault()
        {
            return CalculatedPriceData.IsDefault() && Price.IsDefault() && PnL.IsDefault();
        }
    }

    public interface IForeignCurrencyPositionsUpdater
    {
        void Replace(List<ForeignCurrencyPosition> positions, DateTime date);
        void Update(List<ForeignCurrencyPosition> recalcPositions, DateTime date);
        void Update(ForeignCurrencyPositionData recalcPositions, DateTime date);
    }
}