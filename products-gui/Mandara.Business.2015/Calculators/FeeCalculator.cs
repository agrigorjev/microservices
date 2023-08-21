using Mandara.Data;
using Mandara.Entities;
using Mandara.Entities.Constants;
using Mandara.Entities.EntitiesCustomization;
using System;
using System.Configuration;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Calculators
{
    public class FeeCalculator : IFeeCalculator
    {
        private class MustCalculateRebateInputs
        {
            public bool IsParent { get; private set; }
            public bool IsIceOtc { get; private set; }
            public bool IsNonFutures { get; private set; }
            public bool IsTimeSpread { get; set; }
            public bool IsSell { get; set; }

            public MustCalculateRebateInputs(TradeCapture trade, Product product)
            {
                IsParent = trade.IsParentTrade.True();
                IsIceOtc = IsIceOtcTrade(trade, product);
                IsNonFutures = IsNonFuturesProduct(product);
                IsTimeSpread = IsTimeSpreadStrip(trade.GetStrip(product));
                IsSell = IsSellSide(trade);
            }

            private bool IsIceOtcTrade(TradeCapture trade, Product product)
            {
                string exchangeName = product == null ? "" : product.Exchange.Name;

                return exchangeName.Equals(Exchange.IceExchangeName, StringComparison.InvariantCultureIgnoreCase)
                       && string.IsNullOrEmpty(trade.ExecutingFirm);
            }
            private bool IsNonFuturesProduct(Product product)
            {
                return null != product ? product.Type != ProductType.Futures : false;
            }

            private bool IsTimeSpreadStrip(Strip strip)
            {
                return !strip.IsDefault() && strip.IsTimeSpread;
            }

            private bool IsSellSide(TradeCapture trade)
            {
                return TradeCaptureSide.Sell.GetDescription()
                                       .Equals(trade.Side, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public const decimal BarrelRebateCoef = 0.00009M;
        public const decimal KilotonRebateCoef = 0.001M;
        public const decimal DefaultRebateCoef = 0M;

        public int SpreadRebatePercentage
        {
            get { return Int32.Parse(ConfigurationManager.AppSettings["IceSpreadRebatePercentage"] ?? "50"); }
        }

        public void CalculateBasicFees(TradeCapture trade, Product product)
        {
            trade.FeeClearingDb = CalcClearing(trade, product);
            trade.FeeCommissionDb = CalcCommission(trade, product);
            trade.FeeExchangeDb = CalcExchange(trade, product);
            trade.FeeNfaDb = CalcNfa(trade, product);
            trade.FeeBlockDb = CalcBlock(trade, product);
        }

        public void CalculateFeesForStandardTradeInclRebate(TradeCapture trade)
        {
            CalculateFeesIncludingRebate(trade, trade.SecurityDefinition.Product, CalcIceSpreadRebateForStandardTrade);
        }

        public void CalculateFeesForStandardTradeInclRebate(TradeCapture trade, Product product)
        {
            CalculateFeesIncludingRebate(trade, product, CalcIceSpreadRebateForStandardTrade);
        }

        public void CalculateFeesWithParentSpreadInclRebate(TradeCapture trade)
        {
            CalculateFeesIncludingRebate(trade, trade.SecurityDefinition.Product, CalcIceSpreadRebateWithParentSpread);
        }

        public void CalculateFeesWithParentSpreadInclRebate(TradeCapture trade, Product product)
        {
            CalculateFeesIncludingRebate(trade, product, CalcIceSpreadRebateWithParentSpread);
        }

        private void CalculateFeesIncludingRebate(
            TradeCapture trade,
            Product product,
            Func<TradeCapture, Product, decimal?> calcIceSpreadRebate)
        {
            CalculateBasicFees(trade, product);
            // The ICE spread rebate depends on clearing and exchange
            trade.IceSpreadRebateDb = calcIceSpreadRebate(trade, product);
        }

        private decimal? CalcIceSpreadRebateForStandardTrade(TradeCapture trade, Product product)
        {
            if (!trade.NumOfLots.HasValue)
            {
                return null;
            }

            MustCalculateRebateInputs inputsForRebateCalcDecision = new MustCalculateRebateInputs(trade, product);

            // We need to calculate parent rebate
            if (IsIceOtcNonFuturesParentTimeSpread(inputsForRebateCalcDecision))
            {
                return CalculateIceSpreadRebate(trade, product);
            }

            return null;
        }

        private static decimal? CalculateIceSpreadRebate(TradeCapture trade, Product product)
        {
            decimal currentFees = (trade.FeeClearingDb ?? 0M) + (trade.FeeExchangeDb ?? 0M);
            decimal rebateCoef = 0;

            if (product != null)
            {
                switch (product.Unit.Name.ToLower())
                {
                    case UnitSymbols.Barrel:
                    rebateCoef = BarrelRebateCoef;
                    break;
                    case UnitSymbols.Kiloton:
                    rebateCoef = KilotonRebateCoef;
                    break;
                    default:
                    rebateCoef = DefaultRebateCoef;
                    break;
                }
            }

            return currentFees - trade.NumOfLots * product.ContractSize * rebateCoef;
        }

        private bool IsIceOtcNonFuturesParentTimeSpread(MustCalculateRebateInputs inputsForRebateCalcDecision)
        {

            return inputsForRebateCalcDecision.IsParent &&
                   inputsForRebateCalcDecision.IsIceOtc &&
                   inputsForRebateCalcDecision.IsNonFutures &&
                   inputsForRebateCalcDecision.IsTimeSpread;
        }

        private decimal? CalcIceSpreadRebateWithParentSpread(TradeCapture trade, Product product)
        {
            if (!trade.NumOfLots.HasValue)
            {
                return null;
            }

            MustCalculateRebateInputs inputsForCalcDecision = new MustCalculateRebateInputs(trade, product);

            // We need to calculate sell leg rebate or the parent
            if (IsIceOtcNonFuturesSellLeg(inputsForCalcDecision)
                || IsIceOtcNonFuturesParentTimeSpread(inputsForCalcDecision))
            {
                return CalculateIceSpreadRebate(trade, product);
            }

            return null;
        }

        private bool IsIceOtcNonFuturesSellLeg(MustCalculateRebateInputs inputsForRebateCalcDecision)
        {
            return !inputsForRebateCalcDecision.IsParent &&
                   inputsForRebateCalcDecision.IsSell &&
                   inputsForRebateCalcDecision.IsIceOtc &&
                   inputsForRebateCalcDecision.IsNonFutures;
        }

        public decimal? CalcBlock(TradeCapture trade, Product product)
        {
            if (trade.TradeType != null)
                return null;

            if (product != null)
            {
                decimal? feeBlock = product.FeeBlockTrade;

                if (feeBlock != null && trade.NumOfLots != null)
                    return feeBlock * trade.NumOfLots *
                           (product.FeeConversionFactor.HasValue ? product.FeeConversionFactor : 1);
            }

            return null;
        }

        public decimal? CalcClearing(TradeCapture trade, Product product)
        {
            if (trade.TradeType != null)
                return null;

            // calculate clearing fee
            if (product != null)
            {
                decimal? feeClearing = product.FeeClearing;

                if (feeClearing != null && trade.NumOfLots != null)
                    return feeClearing * trade.NumOfLots *
                           (product.FeeConversionFactor.HasValue ? product.FeeConversionFactor : 1);
            }

            return null;
        }

        public decimal? CalcCommission(TradeCapture trade, Product product)
        {
            if (trade.TradeType != null)
                return null;

            if (product != null)
            {
                decimal? feeCommision = product.FeeCommission;

                if (feeCommision != null && trade.NumOfLots != null)
                    return feeCommision * trade.NumOfLots *
                           (product.FeeConversionFactor.HasValue ? product.FeeConversionFactor : 1);
            }

            return null;
        }

        public decimal? CalcExchange(TradeCapture trade, Product product)
        {
            if (trade.TradeType != null)
                return null;

            if (!string.IsNullOrEmpty(trade.ExecutingFirm))
                return 0M;

            if (product != null)
            {
                decimal? feeExchange = product.FeeExchange;

                if (feeExchange != null && trade.NumOfLots != null)
                    return feeExchange * trade.NumOfLots
                           * (product.FeeConversionFactor.HasValue ? product.FeeConversionFactor : 1);
            }

            return null;
        }

        public decimal? CalcNfa(TradeCapture trade, Product product)
        {
            if (trade.TradeType != null)
                return null;

            if (product != null)
            {
                decimal? feenfa = product.FeeNfa;

                if (feenfa != null && trade.NumOfLots != null)
                    return feenfa * trade.NumOfLots
                           * (product.FeeConversionFactor.HasValue ? product.FeeConversionFactor : 1);
            }

            return null;
        }
    }
}