using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.Managers;
using Mandara.Business.TradeAdd;
using Mandara.Data;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using Mandara.Entities.Enums;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Mandara.Date;
using Mandara.Extensions.Guids;
using Mandara.Extensions.Nullable;

namespace Mandara.Business.Bus
{
    public class TradeAddHandlerConverter : ITradeAddHandlerConverter
    {
        private readonly ITradeAddHandlerConverterProvider _tradeAddHandlerConverterProvider;
        private readonly IOfficialProductToInstrument _officialProductToInstrument;
        private ILogger _log = new NLogLoggerFactory().GetCurrentClassLogger();

        public TradeAddHandlerConverter(ITradeAddHandlerConverterProvider tradeAddHandlerConverterProvider,
            IOfficialProductToInstrument officialProductToInstrument)
        {
            _tradeAddHandlerConverterProvider = tradeAddHandlerConverterProvider;
            _officialProductToInstrument = officialProductToInstrument;
        }

        public TradeDataForTradeAdd GetTradeCaptures(TradeAddDetails tradeAddDetails, bool isMasterToolMode)
        {
            TradeDataForTradeAdd tradeDataForTradeAdd = new TradeDataForTradeAdd();

            if (IsNonFxWithNoStrip1(tradeAddDetails))
            {
                return tradeDataForTradeAdd;
            }

            List<TradeCaptureContainer> tradeCaptureContainers;

            try
            {
                tradeCaptureContainers = ConvertTradeAddDetailsToTradesContainers(tradeAddDetails, isMasterToolMode);
            }
            catch (Exception ex)
            {
                _log.Error(
                    ex,
                    "TradeAddHandlerConverter.GetTradeCaptures - An error occured while converting TradeAddDetails "
                        + "to TradeContainers.");
                tradeDataForTradeAdd.Warnings = new TryGetRef<string> { Value = $"Trade Add Error: {ex.Message}" };
                tradeCaptureContainers = new List<TradeCaptureContainer>();
            }

            if (!tradeCaptureContainers.Any())
            {
                return tradeDataForTradeAdd;
            }

            try
            {
                tradeDataForTradeAdd.TradeCaptures = GetTradeCapturesFromTradeContainers(tradeCaptureContainers);
                tradeDataForTradeAdd.Warnings = GetWarningsFromTradeContainers(tradeCaptureContainers);
                tradeDataForTradeAdd.TransferTradeCaptures = GetTradeCapturesAsTransferTrades(
                    tradeAddDetails,
                    tradeDataForTradeAdd.TradeCaptures);
                SetTradeCapturePortfoliosFromTradeAddDetails(
                    tradeAddDetails,
                    tradeDataForTradeAdd.TradeCaptures,
                    tradeDataForTradeAdd.TransferTradeCaptures);
                tradeDataForTradeAdd.FxTrades = GetFxTrades(tradeCaptureContainers);
            }
            catch (Exception ex)
            {
                _log.Error(
                    ex,
                    "TradeAddHandlerConverter.GetTradeCaptures - an error occurred while getting trade captures from "
                        + "containers or getting transfer trades");
            }

            return tradeDataForTradeAdd;
        }

        private static bool IsNonFxWithNoStrip1(TradeAddDetails tradeAddDetails)
        {
            return (null == tradeAddDetails.StripDetail1 || null == tradeAddDetails.StripDetail1.Instrument)
                   && null == tradeAddDetails.FxSelectedInstrument;
        }

        private List<TradeCapture> GetTradeCapturesFromTradeContainers(
            List<TradeCaptureContainer> tradeCaptureContainers)
        {
            List<TradeCapture> tradeCaptures =
                tradeCaptureContainers.Where(x => x.TradeCapture != null).Select(x => x.TradeCapture).ToList();

            if (IsADiffTrade(tradeCaptures))
            {
                SetParentAndLegsRelationship(tradeCaptures, GetDiffParentAndLegTrades);
            }

            tradeCaptures.ForEach(tc => tc.UpdateStrip());
            return tradeCaptures;
        }

        private static bool IsADiffTrade(List<TradeCapture> tradeCaptures)
        {
            return tradeCaptures.Count == 3;
        }

        private static ParentAndLegTrades GetDiffParentAndLegTrades(List<TradeCapture> inputTrades)
        {
            TradeCapture parent = inputTrades.Single(trade => trade.IsParentTrade.True());
            TradeCapture leg1 = inputTrades.Single(trade => trade.IsParentTrade.False() && (trade.Side == parent.Side));
            TradeCapture leg2 = inputTrades.Single(trade => trade.IsParentTrade.False() && (trade.Side != parent.Side));

            return new ParentAndLegTrades(parent, leg1, leg2);
        }

        private class ParentAndLegTrades
        {
            public TradeCapture Parent { get; private set; }
            public TradeCapture Leg1 { get; private set; }
            public TradeCapture Leg2 { get; private set; }

            public ParentAndLegTrades(TradeCapture parent, TradeCapture leg1, TradeCapture leg2)
            {
                Parent = parent;
                Leg1 = leg1;
                Leg2 = leg2;
            }
        }

        private TradeCapture SetParentAndLegsRelationship(
            List<TradeCapture> tradeCaptures,
            Func<List<TradeCapture>, ParentAndLegTrades> getTradesFromColl)
        {
            _log.Trace(
                "TradeAddHandlerConverter.SetParentAndLegsRelationship - Three trades => parent with buy and sell legs");

            ParentAndLegTrades diffTrades = GetParentAndLegTrades(
                tradeCaptures,
                getTradesFromColl);
            TradeCapture parentTrade = diffTrades.Parent;

            parentTrade.Leg1Trade = diffTrades.Leg1;
            parentTrade.Leg2Trade = diffTrades.Leg2;
            return parentTrade;
        }

        private ParentAndLegTrades GetParentAndLegTrades(
            List<TradeCapture> inputTrades,
            Func<List<TradeCapture>, ParentAndLegTrades> getTradesFromColl)
        {
            try
            {
                return getTradesFromColl(inputTrades);
            }
            catch (InvalidOperationException noSingle)
            {
                _log.Error(
                    noSingle,
                    "TradeAddHandlerConverter.GetParentAndLegTrades - Expected a single parent and one buy and one sell"
                        + "leg.  Possible errors: a) no parent or more than one; b) no buy leg or both buy legs; c) no "
                        + "sell leg or both sell legs.");
                throw;
            }
        }

        private static TryGetResult<string> GetWarningsFromTradeContainers(List<TradeCaptureContainer> tradeCaptureContainers)
        {
            List<string> warningList =
                tradeCaptureContainers.Where(x => !string.IsNullOrEmpty(x.Warning)).Select(x => x.Warning).ToList();

            return new TryGetRef<string>(val => String.IsNullOrWhiteSpace(val))
            {
                Value = string.Join(Environment.NewLine, warningList)
            };
        }

        private List<TradeCapture> GetTradeCapturesAsTransferTrades(
            TradeAddDetails tradeAddDetails,
            List<TradeCapture> tradeCaptures)
        {
            List<TradeCapture> tradeCapturesAsTransferTrades;

            if (tradeAddDetails.TradeType == TradeTypeControl.Transfer)
            {
                tradeCapturesAsTransferTrades = ConvertToTransferTrades(tradeCaptures);

                try
                {
                    tradeCapturesAsTransferTrades.ForEach(tc => tc.UpdateStrip());
                }
                catch (Exception ex)
                {
                    _log.Error(
                        ex,
                        "TradeAddHandlerConverter.GetTradeCapturesAsTransferTrades - a trade Strip update failed.");
                    throw;
                }
            }
            else
            {
                _log.Trace(
                    "TradeAddHandlerConverter.GetTradeCapturesAsTransferTrades - not adding a transfer trade, so "
                        + "no transfer trade captures exist.");
                tradeCapturesAsTransferTrades = new List<TradeCapture>();
            }

            return tradeCapturesAsTransferTrades;
        }

        private List<TradeCapture> ConvertToTransferTrades(List<TradeCapture> tradeCaptures)
        {
            if (tradeCaptures.Count == 1)
            {
                _log.Trace(
                    "TradeAddHandlerConverter.GetTradeCapturesAsTransferTrades - single trade for transfer.  Set "
                    + "it as a transfer sell.");
                tradeCaptures[0].IsTransferSell = true;
            }

            if (IsADiffTrade(tradeCaptures))
            {
                _log.Trace(
                    "TradeAddHandlerConverter.GetTradeCapturesAsTransferTrades - three trades for transfer.  Mark "
                    + "the parent as a transfer sell.");

                try
                {
                    tradeCaptures.Single(trade => trade.IsParentTrade.True()).IsTransferSell = true;
                }
                catch (InvalidOperationException noParentEx)
                {
                    _log.Error(
                        noParentEx,
                        "TradeAddHandlerConverter.GetTradeCapturesAsTransferTrades - expected there to be a "
                            + "single parent in the three trades, but either there's none or more than one");
                    throw;
                }
            }

            List<TradeCapture> transferTrades =
                tradeCaptures.Where(trade => trade != null).Select(CreateTransferTrade).ToList();

            if (IsADiffTrade(transferTrades))
            {
                SetParentAndLegTradesForDiffTransfer(transferTrades);
            }

            return transferTrades;
        }

        private TradeCapture CreateTransferTrade(TradeCapture tradeCapture)
        {
            TradeCapture transferTrade;

            try
            {
                transferTrade = new TradeCapture
                {
                    SecurityDefinition =
                            new SecurityDefinition
                            {
                                Product = tradeCapture.SecurityDefinition.Product,
                                StripName = tradeCapture.SecurityDefinition.StripName,
                                ProductDescription = tradeCapture.SecurityDefinition.ProductDescription,
                                UnderlyingSymbol = GuidExtensions.NumericGuid(GuidExtensions.HalfGuidLength),
                                Exchange = tradeCapture.SecurityDefinition.Exchange,
                                UnderlyingSecurityDesc = tradeCapture.SecurityDefinition.UnderlyingSecurityDesc,
                                StartDate = tradeCapture.SecurityDefinition.StartDate,
                                StartDateAsDate = tradeCapture.SecurityDefinition.StartDateAsDate,
                                UnderlyingMaturityDate = tradeCapture.SecurityDefinition.UnderlyingMaturityDate,
                                HubAlias = tradeCapture.SecurityDefinition.HubAlias
                            },
                    Price = tradeCapture.Price,
                    Quantity = tradeCapture.Quantity * -1M,
                    Side = tradeCapture.Side == "Buy" ? "Sell" : "Buy",
                    TransactTime = tradeCapture.TransactTime,
                    TimeStamp = tradeCapture.TransactTime,
                    TradeStartDate = tradeCapture.TradeStartDate,
                    TradeEndDate = tradeCapture.TradeEndDate,
                    Exchange = tradeCapture.Exchange,
                    ExecutingFirm = tradeCapture.ExecutingFirm,
                    ClearingFirm = tradeCapture.ClearingFirm,
                    TradeDate = tradeCapture.TradeDate,
                    OrdStatus = "Filled",
                    ExecID = GenerateTradeCaptureExecId(),
                    NumOfLots = tradeCapture.NumOfLots,
                    CreatedBy = tradeCapture.CreatedBy,
                    OriginationTrader = tradeCapture.CreatedBy,
                    IsParentTrade = tradeCapture.IsParentTrade,
                    IsParentTimeSpread = tradeCapture.IsParentTimeSpread,
                    TradeType = tradeCapture.TradeType,
                    Pending = tradeCapture.Pending,
                    ExchangeOverride = tradeCapture.ExchangeOverride
                };

                tradeCapture.UtcTransactTime = ConvertLocalTimeToUtc(transferTrade.TransactTime ?? SystemTime.Now());
            }
            catch (Exception ex)
            {
                _log.Error(
                    ex,
                    "TradeAddHandlerConverter..ConvertToTransferTrades - error constructing a new transfer trade "
                        + "from the input trade [{0}]",
                    tradeCapture);
                throw;
            }

            return transferTrade;
        }

        private static void SetLegAndParentRelationship(
            TradeCapture legTrade,
            decimal legPrice,
            TradeCapture parentTrade)
        {
            parentTrade.IsParentTimeSpread = true;
            parentTrade.LegRefID = parentTrade.ExecID;

            legTrade.IsParentTrade = false;
            legTrade.LegRefID = Guid.NewGuid().ToString();
            legTrade.ExecID = parentTrade.ExecID;
            legTrade.ClOrdID = parentTrade.ClOrdID;

            legTrade.Price = legPrice;
        }

        private static void SetTradeCapturePortfoliosFromTradeAddDetails(
            TradeAddDetails tradeAddDetails,
            List<TradeCapture> tradeCaptures,
            List<TradeCapture> tradeCapturesAsTransferTrades)
        {
            if (tradeAddDetails.TradeType != TradeTypeControl.Manual && tradeCapturesAsTransferTrades != null)
            {
                foreach (TradeCapture tradeCapture in tradeCapturesAsTransferTrades)
                {
                    tradeCapture.Portfolio = tradeAddDetails.Portfolio2;
                }
            }
            else
            {
                foreach (TradeCapture tradeCapture in tradeCaptures)
                {
                    tradeCapture.Portfolio = tradeAddDetails.Portfolio1;
                }
            }
        }

        private static List<FxTrade> GetFxTrades(List<TradeCaptureContainer> tradeCaptureContainers)
        {
            List<FxTrade> fxTrades =
                tradeCaptureContainers.Where(x => x.FxTrade != null).Select(x => x.FxTrade).ToList();
            return fxTrades;
        }

        private List<TradeCaptureContainer> ConvertTradeAddDetailsToTradesContainers(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode)
        {
            List<TradeCaptureContainer> tradeCaptureContainers = new List<TradeCaptureContainer>();

            if (tradeAddDetails == null)
            {
                return tradeCaptureContainers;
            }

            if (IsStripTypeFx(tradeAddDetails))
            {
                tradeCaptureContainers.Add(GetFxTradeCapture(tradeAddDetails, isMasterToolMode));
            }

            if (IsStripTypeSingleTrade(tradeAddDetails))
            {
                TradeCaptureContainer tradeCaptureContainer = GetTradeCapture(
                    tradeAddDetails,
                    tradeAddDetails.StripDetail1,
                    false,
                    isMasterToolMode);

                if (tradeCaptureContainer != null)
                {
                    tradeCaptureContainers.Add(tradeCaptureContainer);
                }

                if (IsValidTradeOnCalcPnLOnLegsInstrument(tradeAddDetails, tradeCaptureContainer))
                {
                    CreateLegContainersForCalcPnLOnLegs(
                        tradeAddDetails,
                        isMasterToolMode,
                        tradeCaptureContainer,
                        tradeCaptureContainers);
                }
            }

            if (IsStripTypeSpreadTrade(tradeAddDetails))
            {
                tradeCaptureContainers = CreateTradeContainersForSpread(
                    tradeAddDetails,
                    isMasterToolMode,
                    tradeCaptureContainers);
            }

            return tradeCaptureContainers;
        }

        private static bool IsValidTradeOnCalcPnLOnLegsInstrument(
            TradeAddDetails tradeAddDetails,
            TradeCaptureContainer tradeCaptureContainer)
        {
            return tradeCaptureContainer != null && tradeCaptureContainer.TradeCapture != null
                   && tradeAddDetails.StripDetail1.Instrument.IsCalcPnlFromLegs;
        }

        private void CreateLegContainersForCalcPnLOnLegs(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode,
            TradeCaptureContainer tradeCaptureContainer,
            List<TradeCaptureContainer> tradeCaptureContainers)
        {
            TradeCaptureContainer leg1Container = GetLegTradeCaptureContainer(
                tradeAddDetails,
                false,
                isMasterToolMode,
                tradeCaptureContainer.TradeCapture);
            tradeCaptureContainers.Add(leg1Container);

            TradeCaptureContainer leg2Container = GetLegTradeCaptureContainer(
                tradeAddDetails,
                true,
                isMasterToolMode,
                tradeCaptureContainer.TradeCapture);
            tradeCaptureContainers.Add(leg2Container);

            SetSecurityDefsForCalcPnlFromLegs(tradeCaptureContainer, leg1Container, leg2Container);
            SetLeg2QuantityFromConversionFactor(
                leg2Container,
                tradeCaptureContainer.TradeCapture.SecurityDefinition.Product);
        }

        private TradeCaptureContainer GetLegTradeCaptureContainer(
            TradeAddDetails tradeAddDetails,
            bool isLeg2,
            bool isMasterToolMode,
            TradeCapture trade)
        {
            TradeCaptureContainer legContainer = GetTradeCapture(
                tradeAddDetails,
                tradeAddDetails.StripDetail1,
                isLeg2,
                isMasterToolMode);
            decimal price = !isLeg2 ? tradeAddDetails.StripDetail1.Leg1Price : tradeAddDetails.StripDetail1.Leg2Price;

            SetLegAndParentRelationship(legContainer.TradeCapture, price, trade);
            return legContainer;
        }

        private void SetSecurityDefsForCalcPnlFromLegs(
            TradeCaptureContainer parentContainer,
            TradeCaptureContainer leg1Container,
            TradeCaptureContainer leg2Container)
        {
            Product product = parentContainer.TradeCapture.SecurityDefinition.Product;

            SetLegSecurityDefinition(leg1Container, product.ComplexProduct.ChildProduct1);
            SetLegSecurityDefinition(leg2Container, product.ComplexProduct.ChildProduct2);
        }

        private void SetLegSecurityDefinition(TradeCaptureContainer legContainer, Product legProduct)
        {
            SecurityDefinition legSecDef = legContainer.TradeCapture.SecurityDefinition;

            legSecDef.Product = legProduct;
            legSecDef.ProductDescription = legProduct.OfficialProduct.DisplayName;
            legSecDef.UnderlyingSecurityDesc = GetUnderlyingSecurityDesc(
                legSecDef.StripName,
                legSecDef.ProductDescription);
        }

        private static void SetLeg2QuantityFromConversionFactor(TradeCaptureContainer leg2Container, Product product)
        {
            if (product.ComplexProduct.ConversionFactor2 != null)
            {
                leg2Container.TradeCapture.Quantity = product.ComplexProduct.ConversionFactor2.Value
                                                      * leg2Container.TradeCapture.Quantity;
            }
        }

        private List<TradeCaptureContainer> CreateTradeContainersForSpread(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode,
            List<TradeCaptureContainer> tradeCaptureContainers)
        {
            if (tradeAddDetails.StripDetail1.Instrument.IsCalcPnlFromLegs)
            {
                List<TradeCaptureContainer> invalidTradeContainer = new List<TradeCaptureContainer>
                {
                    new TradeCaptureContainer
                    {
                        Warning = "Entering a time spread for a 'Calculate PnL from Legs' Diff product is not allowed."
                    }
                };

                return invalidTradeContainer;
            }

            TradeCaptureContainer tradeCaptureContainer = GetTradeCapture(
                tradeAddDetails,
                tradeAddDetails.StripDetail1,
                false,
                isMasterToolMode);

            SetUpStripDetail2ForSpread(tradeAddDetails);

            if (tradeCaptureContainer.TradeCapture != null)
            {
                AddSpreadTradeToContainers(
                    tradeCaptureContainers,
                    tradeCaptureContainer,
                    tradeAddDetails,
                    isMasterToolMode);
            }
            else
            {
                tradeCaptureContainers.Add(tradeCaptureContainer);
            }

            return tradeCaptureContainers;
        }

        private void AddSpreadTradeToContainers(
            List<TradeCaptureContainer> tradeCaptureContainers,
            TradeCaptureContainer tradeCaptureContainer,
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode)
        {
            ProductDateType dateType1 = ConvertStripToProductDateType(tradeAddDetails.StripDetail1.Strip);
            ProductDateType dateType2 = ConvertStripToProductDateType(tradeAddDetails.StripDetail2.Strip);

            if ((dateType1 == ProductDateType.Day) && (dateType2 == ProductDateType.Day))
            {
                // bal/bal => 2 trades
                TradeCaptureContainer tradeCaptureContainer2 = GetTradeCapture(
                    tradeAddDetails,
                    tradeAddDetails.StripDetail2,
                    true,
                    isMasterToolMode);

                tradeCaptureContainers.Add(tradeCaptureContainer);
                tradeCaptureContainers.Add(tradeCaptureContainer2);

                if (tradeCaptureContainer2.TradeCapture == null)
                {
                    return;
                }

                if (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                {
                    tradeCaptureContainer.TradeCapture.Price = tradeAddDetails.StripDetail1.Leg1Price;
                    tradeCaptureContainer2.TradeCapture.Price = tradeAddDetails.StripDetail1.Leg2Price;
                }
            }
            else if (dateType1 == dateType2)
            {
                if (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                {
                    // (m/q/cal) / (m/q/cal) => 1 trade
                    tradeCaptureContainer.TradeCapture.SecurityDefinition.StripName += "/"
                                                                                       + tradeAddDetails
                                                                                           .StripDetail2.Strip
                                                                                           .StringValue;
                    tradeCaptureContainers.Add(tradeCaptureContainer);

                    // adding time spread legs
                    TradeCaptureContainer tradeCaptureContainer1 = GetTradeCapture(
                        tradeAddDetails,
                        tradeAddDetails.StripDetail1,
                        false,
                        isMasterToolMode);
                    SetLegAndParentRelationship(
                        tradeCaptureContainer1.TradeCapture,
                        tradeAddDetails.StripDetail1.Leg1Price,
                        tradeCaptureContainer.TradeCapture);
                    tradeCaptureContainers.Add(tradeCaptureContainer1);

                    TradeCaptureContainer tradeCaptureContainer2 = GetTradeCapture(
                        tradeAddDetails,
                        tradeAddDetails.StripDetail2,
                        true,
                        isMasterToolMode);
                    SetLegAndParentRelationship(
                        tradeCaptureContainer2.TradeCapture,
                        tradeAddDetails.StripDetail1.Leg2Price,
                        tradeCaptureContainer.TradeCapture);
                    tradeCaptureContainers.Add(tradeCaptureContainer2);
                }
                else
                {
                    TradeCaptureContainer tradeCaptureContainer2 = GetTradeCapture(
                        tradeAddDetails,
                        tradeAddDetails.StripDetail2,
                        true,
                        isMasterToolMode);

                    tradeCaptureContainers.Add(tradeCaptureContainer);
                    tradeCaptureContainers.Add(tradeCaptureContainer2);

                    if (tradeCaptureContainer2.TradeCapture == null)
                    {
                        return;
                    }
                }
            }
            else if (dateType1 != dateType2)
            {
                // m/bal / (q/cal) => 2 trades with differnet quantities
                TradeCaptureContainer tradeCaptureContainer2 = GetTradeCapture(
                    tradeAddDetails,
                    tradeAddDetails.StripDetail2,
                    true,
                    isMasterToolMode);

                tradeCaptureContainers.Add(tradeCaptureContainer);
                tradeCaptureContainers.Add(tradeCaptureContainer2);

                if (tradeCaptureContainer2.TradeCapture == null)
                {
                    return;
                }

                ProductDateType max = dateType1;
                ProductDateType min = dateType2;

                if ((dateType2 == ProductDateType.Year)
                    || ((dateType2 == ProductDateType.Quarter)
                        && ((dateType1 == ProductDateType.MonthYear) || (dateType1 == ProductDateType.Day))))
                {
                    max = dateType2;
                    min = dateType1;
                }

                decimal coefficient = 1M;

                if (((min == ProductDateType.MonthYear) || (min == ProductDateType.Day))
                    && (max == ProductDateType.Quarter))
                {
                    coefficient = 3M;
                }
                if (((min == ProductDateType.MonthYear) || (min == ProductDateType.Day))
                    && (max == ProductDateType.Year))
                {
                    coefficient = 12M;
                }
                if ((min == ProductDateType.Quarter) && (max == ProductDateType.Year))
                {
                    coefficient = 4M;
                }

                if (max == dateType1)
                {
                    tradeCaptureContainer2.TradeCapture.Quantity *= coefficient;
                }
                else
                {
                    tradeCaptureContainer2.TradeCapture.Quantity /= coefficient;
                }

                if (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                {
                    tradeCaptureContainer.TradeCapture.Price = tradeAddDetails.StripDetail1.Leg1Price;
                    tradeCaptureContainer2.TradeCapture.Price = tradeAddDetails.StripDetail1.Leg2Price;
                }
            }
        }

        private void SetUpStripDetail2ForSpread(TradeAddDetails tradeAddDetails)
        {
            if (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
            {
                tradeAddDetails.StripDetail2.Instrument = tradeAddDetails.StripDetail1.Instrument;
                tradeAddDetails.StripDetail2.Price = tradeAddDetails.StripDetail1.Price;
                tradeAddDetails.StripDetail2.Unit = tradeAddDetails.StripDetail1.Unit;
            }

            tradeAddDetails.StripDetail2.Volume = tradeAddDetails.StripDetail1.Volume;
        }

        private bool IsStripTypeSpreadTrade(TradeAddDetails tradeAddDetails)
        {
            return ((tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                    || (tradeAddDetails.StripTypeControl == StripTypeControl.FutureVsSwap))
                   && (tradeAddDetails.StripDetail1 != null) && (tradeAddDetails.StripDetail2 != null);
        }

        private bool IsStripTypeSingleTrade(TradeAddDetails tradeAddDetails)
        {
            return ((tradeAddDetails.StripTypeControl == StripTypeControl.Flat)
                    || (tradeAddDetails.StripTypeControl == StripTypeControl.CustomMonthly)
                    || (tradeAddDetails.StripTypeControl == StripTypeControl.DailySwap)
                    || (tradeAddDetails.StripTypeControl == StripTypeControl.DailyDiff))
                   && (tradeAddDetails.StripDetail1 != null);
        }

        private bool IsStripTypeFx(TradeAddDetails tradeAddDetails)
        {
            return tradeAddDetails.StripTypeControl == StripTypeControl.FX;
        }

        private static ProductDateType ConvertStripToProductDateType(Strip strip)
        {
            if (strip.IsBalmoStrip)
            {
                return ProductDateType.Day;
            }

            if (strip.StringValue.ToLower().Contains("cal"))
            {
                return ProductDateType.Year;
            }

            if (strip.StringValue.ToLower().Contains("q"))
            {
                return ProductDateType.Quarter;
            }

            return ProductDateType.MonthYear;
        }

        private TradeCaptureContainer GetFxTradeCapture(TradeAddDetails tradeAddDetails, bool isMasterToolMode)
        {
            if (tradeAddDetails.StripTypeControl != StripTypeControl.FX)
            {
                return null;
            }

            Product fxProduct = GetFxProduct(tradeAddDetails, isMasterToolMode);

            if (fxProduct == null)
            {
                return TradeAddHandlerConverter.ConstructMissingFxProductWarning(tradeAddDetails);
            }

            Func<TradeAddDetails, TradeCapture> tradeCaptureCreator;
            Func<TradeAddDetails, Product, TradeCapture, FxTrade> fxTradeCreator;

            if (isMasterToolMode)
            {
                tradeCaptureCreator = CreateTradeMasterToolMode;
                fxTradeCreator = CreateFxTradeMasterToolMode;
            }
            else
            {
                tradeCaptureCreator = CreateTrade;
                fxTradeCreator = CreateFxTrade;
            }

            return CreateTradeCaptureContainer(
                tradeAddDetails,
                fxProduct,
                tradeCaptureCreator,
                fxTradeCreator);
        }

        private static TradeCaptureContainer ConstructMissingFxProductWarning(TradeAddDetails tradeAddDetails)
        {
            string warningMsg =
                String.Format(
                    "The selected instrument cannot be found. Please pass the technical information below to "
                        + "trade support:\r\nExchange: {0}\r\nTrade Type: {1}\r\nStrip Type: {2}\r\n"
                        + "FxInstrument: {3}\r\n",
                    tradeAddDetails.Exchange,
                    tradeAddDetails.TradeType,
                    tradeAddDetails.StripTypeControl,
                    tradeAddDetails.FxSelectedInstrument);

            return new TradeCaptureContainer { Warning = warningMsg };
        }

        private TradeCaptureContainer CreateTradeCaptureContainer(
            TradeAddDetails tradeAddDetails,
            Product fxProduct,
            Func<TradeAddDetails, TradeCapture> createTrade,
            Func<TradeAddDetails, Product, TradeCapture, FxTrade> createFxTrade)
        {
            TradeCapture tradeCapture = createTrade(tradeAddDetails);
            FxTrade fxTrade = createFxTrade(tradeAddDetails, fxProduct, tradeCapture);

            return new TradeCaptureContainer { TradeCapture = tradeCapture, FxTrade = fxTrade };
        }

        // TODO: Apparently some fields are set in master tool mode and others are set when not in master tool mode 
        // (this isn't entirely clear from the MasterToolMode instance of this method since it's checking if the values
        // have been set, implying that they may well not be set anyway).  The implication is that either the client
        // needs to set a consistent set of fields based on the context and the user just uses them or the distinction
        // between the fields used and why they're used must be explicit as it is in the code here at the moment.
        private TradeCapture CreateTrade(TradeAddDetails tradeAddDetails)
        {
            TradeCapture tradeCapture = CreateBaseTradeCaptureFromTradeDetail(tradeAddDetails);

            tradeCapture.UtcTransactTime = tradeAddDetails.CreatedAtUtc;

            DateTime localTransactTime = tradeAddDetails.CreatedAtUtc.ToLocalTime();

            tradeCapture.TradeDate = localTransactTime.Date;
            tradeCapture.TransactTime = tradeCapture.TimeStamp = localTransactTime;
            return tradeCapture;
        }

        private TradeCapture CreateBaseTradeCaptureFromTradeDetail(TradeAddDetails tradeAddDetails)
        {
            TradeCapture tradeCapture = new TradeCapture();
            tradeCapture.SecurityDefinition = _tradeAddHandlerConverterProvider.GetFxTradesSecurityDefinition();
            tradeCapture.Side = tradeAddDetails.Side == SideControl.Buy ? "Buy" : "Sell";
            tradeCapture.Exchange = tradeAddDetails.Exchange;
            tradeCapture.ExecutingFirm = tradeAddDetails.Broker ?? string.Empty;
            tradeCapture.OrdStatus = "Filled";
            tradeCapture.ExecID = GenerateTradeCaptureExecId();
            tradeCapture.CreatedBy = tradeCapture.OriginationTrader = tradeAddDetails.CreatedByUserName;
            tradeCapture.IsParentTrade = true;
            tradeCapture.TradeType = tradeAddDetails.TradeType == TradeTypeControl.Manual ? 2 : 1;
            tradeCapture.Pending = false;
            return tradeCapture;
        }

        private TradeCapture CreateTradeMasterToolMode(TradeAddDetails tradeAddDetails)
        {
            TradeCapture tradeCapture = CreateBaseTradeCaptureFromTradeDetail(tradeAddDetails);

            if (tradeAddDetails.TradeDate.HasValue)
            {
                tradeCapture.TradeDate = tradeAddDetails.TradeDate.Value.Date;
            }

            if (tradeAddDetails.TimestampUtc.HasValue)
            {
                tradeCapture.TimeStamp = tradeAddDetails.TimestampUtc.Value;
            }

            if (tradeAddDetails.TransactTimeUtc.HasValue)
            {
                tradeCapture.TransactTime = tradeAddDetails.TransactTimeUtc.Value.ToLocalTime();
                tradeCapture.UtcTransactTime = tradeAddDetails.TransactTimeUtc.Value;
            }

            return tradeCapture;
        }

        private FxTrade CreateFxTrade(
            TradeAddDetails tradeAddDetails,
            Product fxProduct,
            TradeCapture tradeCapture)
        {
            FxTrade fxTrade = new FxTrade();
            SetSpecifiedAndAgainstCurrency(fxTrade, tradeAddDetails, fxProduct);
            fxTrade.Product = fxProduct;
            fxTrade.TradeCapture = tradeCapture;
            fxTrade.ProductType = tradeAddDetails.IsSpot ? FxProductTypes.Spot : FxProductTypes.Forward;
            fxTrade.Rate = tradeAddDetails.FxExchangeRate;
            fxTrade.SpotRate = tradeAddDetails.FxExchangeRate;

            fxTrade.Tenor = tradeAddDetails.IsSpot
                ? FxProductTypes.Spot
                : tradeAddDetails.ForwardValueDate.ToDayFirstString();
            fxTrade.LinkTradeReportId = tradeCapture.ExecID;
            fxTrade.LinkType = "Parent";
            fxTrade.ValueDate = tradeAddDetails.IsSpot ? SystemTime.Today() : tradeAddDetails.ForwardValueDate;
            fxTrade.SpecifiedAmount = tradeAddDetails.SpecifiedAmount;
            fxTrade.AgainstAmount = tradeAddDetails.AgainstAmount;

            return fxTrade;
        }

        private FxTrade CreateFxTradeMasterToolMode(
            TradeAddDetails tradeAddDetails,
            Product fxProduct,
            TradeCapture tradeCapture)
        {
            FxTrade fxTrade = CreateFxTrade(tradeAddDetails, fxProduct, tradeCapture);

            if (tradeAddDetails.IsSpot)
            {
                fxTrade.ValueDate = tradeAddDetails.TradeDate.Value;
            }

            return fxTrade;
        }

        private void SetSpecifiedAndAgainstCurrency(
            FxTrade fxTrade,
            TradeAddDetails tradeAddDetails,
            Product fxProduct)
        {
            if (fxProduct.Currency1.IsoName == tradeAddDetails.FxSelectedInstrument.FxSpecifiedCurrency)
            {
                fxTrade.SpecifiedCurrency = fxProduct.Currency1;
                fxTrade.AgainstCurrency = fxProduct.Currency2;
            }
            else
            {
                fxTrade.SpecifiedCurrency = fxProduct.Currency2;
                fxTrade.AgainstCurrency = fxProduct.Currency1;
            }
        }

        private string GenerateTradeCaptureExecId()
        {
            return GuidExtensions.NumericGuid(GuidExtensions.HalfGuidLength);
        }

        private TradeCaptureContainer GetTradeCapture(
            TradeAddDetails tradeAddDetails,
            StripDetail stripDetail,
            bool secondTrade,
            bool isMasterToolMode)
        {
            SideControl sideControl = tradeAddDetails.Side;

            if (secondTrade)
            {
                sideControl = sideControl == SideControl.Buy ? SideControl.Sell : SideControl.Buy;
            }

            Product product = GetProduct(
                tradeAddDetails,
                stripDetail,
                secondTrade,
                isMasterToolMode);

            if (product == null)
            {
                return ConstructMissingNonFxProductWarning(tradeAddDetails, stripDetail);
            }

            string exchange = tradeAddDetails.IsInternalExchange ? product.Exchange.Name : tradeAddDetails.Exchange;

            TradeCapture tradeCapture = new TradeCapture();
            SecurityDefinition securityDefinition = new SecurityDefinition();
            securityDefinition.Product = product;
            securityDefinition.StripName = stripDetail.Strip.IsBalmoStrip ? "Bal Month" : stripDetail.Strip.StringValue;

            securityDefinition.ProductDescription = product.OfficialProduct.DisplayName;
            securityDefinition.UnderlyingSymbol = GenerateTradeCaptureExecId();
            securityDefinition.Exchange = exchange;
            securityDefinition.UnderlyingSecurityDesc = GetUnderlyingSecurityDesc(
                securityDefinition.StripName,
                securityDefinition.ProductDescription);
            securityDefinition.StartDate = stripDetail.Strip.StartDate.ToSortableShortDate();
            securityDefinition.StartDateAsDate = stripDetail.Strip.StartDate;
            securityDefinition.EndDate = stripDetail?.Strip.EndDate?.ToSortableShortDate();
            securityDefinition.EndDateAsDate = stripDetail.Strip.EndDate;
            securityDefinition.UnderlyingMaturityDate = tradeAddDetails.CreatedAtUtc.Date.ToSortableShortDate();

            tradeCapture.SecurityDefinition = securityDefinition;

            tradeCapture.Price = stripDetail.Price;
            tradeCapture.Quantity = stripDetail.Volume * (sideControl == SideControl.Buy ? 1M : -1M);
            tradeCapture.Side = sideControl == SideControl.Buy ? "Buy" : "Sell";
            tradeCapture.TradeStartDate = stripDetail.Strip.StartDate;
            tradeCapture.TradeEndDate = stripDetail.Strip.EndDate;

            tradeCapture.Exchange = exchange;
            tradeCapture.ExecutingFirm = tradeAddDetails.Broker ?? string.Empty;
            tradeCapture.OrdStatus = "Filled";
            tradeCapture.ExecID = GenerateTradeCaptureExecId();

            int numberOfMonthsFromStripName;

            if (tradeCapture.SecurityDefinition.Product.IsProductDaily)
            {
                numberOfMonthsFromStripName = 1;
            }
            else
            {
                numberOfMonthsFromStripName =
                    Client.Managers.LiveDataManager.GetNumberOfMonthsFromStripName(
                        tradeCapture.SecurityDefinition.StripName,
                        tradeCapture.TradeStartDate,
                        tradeCapture.TradeEndDate);
            }

            tradeCapture.NumOfLots = Convert.ToInt32(
                Math.Abs(tradeCapture.Quantity.Value) * numberOfMonthsFromStripName);
            tradeCapture.CreatedBy = tradeCapture.OriginationTrader = tradeAddDetails.CreatedByUserName;
            tradeCapture.IsParentTrade = true;
            tradeCapture.TradeType = tradeAddDetails.TradeType == TradeTypeControl.Manual ? 2 : 1;
            tradeCapture.Pending = false;
            tradeCapture.ExchangeOverride = tradeAddDetails.IsInternalExchange ? "Internal" : null;

            DateTime localCreationTime = tradeAddDetails.CreatedAtUtc.ToLocalTime();

            if (tradeAddDetails.IsMasterToolMode)
            {
                tradeCapture.TradeDate = tradeAddDetails.TradeDate?.Date ?? localCreationTime.Date;
                tradeCapture.TimeStamp = tradeAddDetails.TimestampUtc?.ToLocalTime() ?? localCreationTime;
                tradeCapture.TransactTime = tradeAddDetails.TransactTimeUtc?.ToLocalTime() ?? localCreationTime;
            }
            else
            {
                tradeCapture.TradeDate = localCreationTime.Date;
                tradeCapture.TransactTime = tradeCapture.TimeStamp = localCreationTime;
            }

            tradeCapture.UtcTransactTime = tradeCapture.TransactTime.Value.ToUniversalTime();

            // TODO: What happens if the TradeCapture.Strip is default (previously null)?
            tradeCapture.SecurityDefinition.Strip1Date = tradeCapture.Strip.Part1.StartDate;
            tradeCapture.SecurityDefinition.Strip1DateType = tradeCapture.Strip.Part1.DateType;

            if (tradeCapture.Strip.IsTimeSpread)
            {
                tradeCapture.SecurityDefinition.Strip2Date = tradeCapture.Strip.Part2.StartDate;
                tradeCapture.SecurityDefinition.Strip2DateType = tradeCapture.Strip.Part2.DateType;
            }

            return new TradeCaptureContainer { TradeCapture = tradeCapture };
        }

        private static DateTime ConvertLocalTimeToUtc(DateTime localDateTime)
        {
            return DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime();
        }

        private static TradeCaptureContainer ConstructMissingNonFxProductWarning(
            TradeAddDetails tradeAddDetails,
            StripDetail stripDetail)
        {
            string warningMsg =
                String.Format(
                    "The selected instrument cannot be found. Please pass the technical information below to trade "
                        + "support:\r\nExchange: {0}\r\nTrade Type: {1}\r\nStrip Type: {2}\r\nInstrument: {3}\r\n"
                        + "TAS option:  {4}\r\n" + "Units: {5}\r\nStrip: {6}\r\n",
                    tradeAddDetails.Exchange,
                    tradeAddDetails.TradeType,
                    tradeAddDetails.StripTypeControl,
                    stripDetail.Instrument,
                    (tradeAddDetails.IsTasChecked.HasValue ? tradeAddDetails.IsTasChecked.ToString() : "n/a"),
                    stripDetail.Unit,
                    stripDetail);

            return new TradeCaptureContainer
            {
                Warning = warningMsg
            };
        }

        private class TradeCaptureContainer
        {
            public TradeCapture TradeCapture { get; set; }
            public FxTrade FxTrade { get; set; }
            public string Warning { get; set; }
        }

        private Product GetFxProduct(TradeAddDetails tradeAddDetails, bool isMasterToolMode)
        {
            if (tradeAddDetails.FxSelectedInstrument == null)
            {
                return null;
            }

            Dictionary<Instrument, OfficialProduct> instrumentsProductsMap = GetInstrumentsProductsMap(isMasterToolMode);
            Instrument instrument =
                instrumentsProductsMap.Keys.FirstOrDefault(x => x.Id == tradeAddDetails.FxSelectedInstrument.Id);

            if (instrument == null)
            {
                return null;
            }

            OfficialProduct officialProduct;
            if (!instrumentsProductsMap.TryGetValue(instrument, out officialProduct))
            {
                return null;
            }

            List<Product> products;
            products =
                officialProduct.Products.Where(
                    x =>
                        (x.Exchange != null)
                        && x.Exchange.Name.Equals(tradeAddDetails.Exchange, StringComparison.InvariantCultureIgnoreCase))
                               .ToList();

            if (!isMasterToolMode)
            {
                products = products.Where(x => x.IsAllowedForManualTrades).ToList();
            }

            return products.FirstOrDefault();
        }

        private Product GetProduct(
            TradeAddDetails tradeAddDetails,
            StripDetail stripDetail,
            bool secondTrade,
            bool isMasterToolMode)
        {
            DateTime now = SystemTime.Now();

            // from product tool trade impact (this product is just being edited in the product tool)
            if (tradeAddDetails.IsProductFromProductTool)
            {
                return GetProductToolProduct(tradeAddDetails, isMasterToolMode, now);
            }

            Dictionary<Instrument, OfficialProduct> instrumentsProductsMap = GetInstrumentsProductsMap(isMasterToolMode);
            Instrument instrument = instrumentsProductsMap.Keys.FirstOrDefault(x => x.Id == stripDetail.Instrument.Id);

            if (instrument == null)
            {
                return null;
            }

            OfficialProduct officialProduct;
            if (!instrumentsProductsMap.TryGetValue(instrument, out officialProduct))
            {
                return null;
            }

            // first trade for "Futures vs Swap" is ought to be futures
            if (!secondTrade && (tradeAddDetails.StripTypeControl == StripTypeControl.FutureVsSwap))
            {
                ICollection<Product> productsCollection = officialProduct.Products;

                if (!isMasterToolMode)
                {
                    productsCollection = productsCollection.Where(x => x.IsAllowedForManualTrades).ToList();
                }

                if (tradeAddDetails.IsTasChecked.HasValue && tradeAddDetails.IsTasChecked.Value)
                {
                    productsCollection = productsCollection.Where(x => x.TasType == TasType.Tas).ToList();
                }
                else if (tradeAddDetails.IsMopsChecked.HasValue && tradeAddDetails.IsMopsChecked.Value)
                {
                    productsCollection = productsCollection.Where(x => x.TasType == TasType.Mops).ToList();
                }
                else if (tradeAddDetails.IsMmChecked.HasValue && tradeAddDetails.IsMmChecked.Value)
                {
                    productsCollection = productsCollection.Where(x => x.TasType == TasType.Mm).ToList();
                }
                else
                {
                    productsCollection = productsCollection.Where(x => x.TasType == TasType.NotTas).ToList();
                }

                // priority to contract size 1000
                Product futures =
                    productsCollection.FirstOrDefault(x => (x.Type == ProductType.Futures) && (x.ContractSize == 1000M));

                if (futures == null)
                {
                    futures = productsCollection.FirstOrDefault(x => x.Type == ProductType.Futures);
                }

                return futures;
            }

            IEnumerable<Product> products;

            if (tradeAddDetails.IsInternalExchange)
            {
                // filter by products by IsTransferProduct flag
                products = officialProduct.Products.Where(x => x.IsInternalTransferProductDb == true);

                // filter by expiry exchange
                if (!string.IsNullOrEmpty(tradeAddDetails.ExpiryExchange))
                {
                    products =
                        products.Where(
                            x =>
                                (x.Exchange != null)
                                && x.Exchange.Name.Equals(
                                    tradeAddDetails.ExpiryExchange,
                                    StringComparison.InvariantCultureIgnoreCase));
                }
            }
            else
            {
                // filter by exchange
                products =
                    officialProduct.Products.Where(
                        x =>
                            (x.Exchange != null)
                            && x.Exchange.Name.Equals(
                                tradeAddDetails.Exchange,
                                StringComparison.InvariantCultureIgnoreCase));
            }

            // filter by unit
            products = products.Where(x => x.Unit.UnitId == stripDetail.Unit.UnitId);

            // filter by balmo
            if (stripDetail.Strip.IsBalmoStrip)
            {
                IEnumerable<Product> list = products;

                products = list.Where(x => x.Type == ProductType.Balmo);

                if (!products.Any())
                {
                    products = list.Where(x => x.Type == ProductType.Swap);
                }
            }
            else
            {
                products = products.Where(x => x.Type != ProductType.Balmo);
            }

            products = FilterProductsForStripTypeControl(tradeAddDetails, products);
            products = FilterForTasProductTypes(tradeAddDetails, products);

            if (!isMasterToolMode)
            {
                products = products.Where(x => x.IsAllowedForManualTrades).ToList();
            }

            // priority to contract size 1000
            Product product = products.FirstOrDefault(x => x.ContractSize == 1000M);

            if (product == null)
            {
                product = products.FirstOrDefault();
            }

            return product;
        }

        private Product GetProductToolProduct(
            TradeAddDetails tradeAddDetails,
            bool isMasterToolMode,
            DateTime now)
        {
            TryGetResult<OfficialProduct> officialProduct =
                _tradeAddHandlerConverterProvider.GetValidOfficialProduct(tradeAddDetails.OfficialProductId, now);

            if (!officialProduct.HasValue || (!isMasterToolMode && !officialProduct.Value.IsAllowedForManualTrades))
            {
                return null;
            }

            tradeAddDetails.Product.OfficialProduct = officialProduct.Value;
            return tradeAddDetails.Product;
        }

        private static IEnumerable<Product> FilterForTasProductTypes(
            TradeAddDetails tradeAddDetails,
            IEnumerable<Product> products)
        {
            IEnumerable<Product> filteredForTasTypes;

            // if TAS/MOPS/MM checkbox was set we are looking at TAS/MOPS/MM trades only
            if (tradeAddDetails.IsTasChecked.True())
            {
                filteredForTasTypes = products.Where(x => x.TasType == TasType.Tas);
            }
            else if (tradeAddDetails.IsMopsChecked.True())
            {
                filteredForTasTypes = products.Where(x => x.TasType == TasType.Mops);
            }
            else if (tradeAddDetails.IsMmChecked.True())
            {
                filteredForTasTypes = products.Where(x => x.TasType == TasType.Mm);
            }
            else if (tradeAddDetails.IsMocChecked.True())
            {
                filteredForTasTypes = products.Where(product =>
                    TasType.Tas == product.TasType && ProductType.Swap == product.Type);
            }
            else
            {
                filteredForTasTypes = products.Where(x => x.TasType == TasType.NotTas);
            }

            return filteredForTasTypes;
        }

        private static IEnumerable<Product> FilterProductsForStripTypeControl(
            TradeAddDetails tradeAddDetails,
            IEnumerable<Product> products)
        {
            IEnumerable<Product> filteredProducts;

            switch (tradeAddDetails.StripTypeControl)
            {
                case StripTypeControl.DailySwap:
                {
                    filteredProducts = products.Where(prod => prod.Type == ProductType.DailySwap);
                }
                break;

                case StripTypeControl.DailyDiff:
                {
                    filteredProducts = FilterForDailyDiffProducts(tradeAddDetails.StripDetail1.Strip, products);
                }
                break;

                default:
                {
                    filteredProducts = products;
                }
                break;
            }

            return filteredProducts;
        }

        private static IEnumerable<Product> FilterForDailyDiffProducts(
            Strip stripForDates,
            IEnumerable<Product> products)
        {
            HashSet<ProductType> allowedTypes =
                new HashSet<ProductType>() { ProductType.DayVsMonthCustom, ProductType.DailyVsDaily };

            // Friday - Monday = 4 days.  E.g. 20 - 16.
            if (DayOfWeek.Monday == stripForDates.StartDate.DayOfWeek
                && TimeSpan.FromDays(4) == stripForDates.EndDate - stripForDates.StartDate)
            {
                allowedTypes.Add(ProductType.DayVsMonthFullWeek);
            }

            return products.Where(prod => allowedTypes.Contains(prod.Type));
        }

        private void SetParentAndLegTradesForDiffTransfer(List<TradeCapture> transferTrades)
        {
            TradeCapture parent = SetParentAndLegsRelationship(transferTrades, GetTransferParentAndLegTrades);

            SetLegAndParentRelationship(parent.Leg1Trade, parent.Leg1Trade.Price.Value, parent);
            SetLegAndParentRelationship(parent.Leg2Trade, parent.Leg2Trade.Price.Value, parent);
        }

        private static ParentAndLegTrades GetTransferParentAndLegTrades(List<TradeCapture> inputTrades)
        {
            TradeCapture parent = inputTrades.Single(trade => trade.IsParentTrade.True());
            TradeCapture leg1 =
                inputTrades.Single(
                    trade => trade.IsParentTrade.False() && (trade.Side == TradeCaptureSide.Buy.GetDescription()));
            TradeCapture leg2 =
                inputTrades.Single(
                    trade => trade.IsParentTrade.False() && (trade.Side == TradeCaptureSide.Sell.GetDescription()));

            return new ParentAndLegTrades(parent, leg1, leg2);
        }

        private string GetUnderlyingSecurityDesc(string stripName, string productDescription)
        {
            string underlyingSecurityDesc = productDescription + " - " + stripName;
            int length = underlyingSecurityDesc.Length;

            if (length > 65)
            {
                int prodDescLength = productDescription.Length;

                string truncatedProductDescription = productDescription.Remove(prodDescLength - (length - 65));
                underlyingSecurityDesc = truncatedProductDescription + " - " + stripName;
            }
            return underlyingSecurityDesc;
        }

        public Dictionary<Instrument, OfficialProduct> GetInstrumentsProductsMap(bool isMasterToolMode)
        {
            List<OfficialProduct> officialProducts;
            officialProducts = _tradeAddHandlerConverterProvider.GetOfficialProducts();

            DateTime now = SystemTime.Now();

            List<OfficialProduct> offProducts = officialProducts.ToList();

            if (!isMasterToolMode)
            {
                offProducts = officialProducts.Where(x => x.IsAllowedForManualTrades).ToList();
            }

            foreach (OfficialProduct offProduct in offProducts)
            {
                offProduct.Products =
                    offProduct.Products.Where(
                        it =>
                            ((it.ValidFrom == null) || (it.ValidFrom <= now))
                            && ((it.ValidTo == null) || (it.ValidTo >= now))).ToList();

                if (!isMasterToolMode)
                {
                    offProduct.Products = offProduct.Products.Where(it => it.IsAllowedForManualTrades).ToList();
                }
            }

            return
                offProducts.ToDictionary(
                    officialProduct =>
                    {
                        if (isMasterToolMode)
                        {
                            return
                                _officialProductToInstrument.ConvertOfficialProductToInstrumentForMasterToolMode(
                                    officialProduct);
                        }
                        else
                        {
                            return _officialProductToInstrument.ConvertOfficialProductToInstrument(officialProduct);
                        }
                    },
                    value => value);
        }

        public Action<TradeAddDetails, TradeCapture, IEnumerable<int>> GetActionSetTradeDetailsFromParent(
            bool isDuplicateMode)
        {
            if (!isDuplicateMode)
            {
                return SetNonDuplicateModeTradeAddDetails;
            }

            return (tradeDetails, parentTrade, trades) => { };
        }

        public TryGetResult<TradeAddDetails> ConvertTradeCaptureToTradeAddDetails(
            Func<int, TryGetResult<OfficialProduct>> getOfficialProductForSecDef,
            Func<int, TryGetResult<FxTrade>> getFxTrade,
            List<TradeCapture> tradeCaptures,
            Action<TradeAddDetails, TradeCapture, IEnumerable<int>> setTradeDetailsFromParent)
        {
            TryGetResult<TradeAddDetails> tradeAdd = ConvertTradeCaptureToTradeAddDetails(
                getOfficialProductForSecDef,
                getFxTrade,
                tradeCaptures.Select(trade => new TradePieces(trade)).ToList(),
                setTradeDetailsFromParent);

            return tradeAdd;
        }

        private static void SetNonDuplicateModeTradeAddDetails(
            TradeAddDetails tradeAddDetails,
            TradeCapture firstParentTrade,
            IEnumerable<int> tradeIds)
        {
            tradeAddDetails.TradeCaptureIds = tradeIds.ToList();
            tradeAddDetails.GroupId = firstParentTrade.GroupId;
            tradeAddDetails.TradeDate = firstParentTrade.TradeDate;
            tradeAddDetails.TimestampUtc = firstParentTrade.TimeStamp?.ToUniversalTime();
            tradeAddDetails.TransactTimeUtc = firstParentTrade.TransactTime?.ToUniversalTime();
        }

        private static TradeAddDetails SetTimeSpreadAddDetailsPrices(TradeAddDetails tradeAddDetails)
        {
            tradeAddDetails.StripDetail1.Leg1Price = tradeAddDetails.StripDetail1.Price;
            tradeAddDetails.StripDetail1.Leg2Price = tradeAddDetails.StripDetail2.Price;
            tradeAddDetails.StripDetail1.Price = tradeAddDetails.StripDetail1.Leg1Price
                                                 - tradeAddDetails.StripDetail1.Leg2Price;
            return tradeAddDetails;
        }

        private static TradeAddDetails SetTransferTradePortfolio(TradeAddDetails tradeAddDetails, TradeCapture trade)
        {
            tradeAddDetails.Portfolio1 = trade.SellBook;
            tradeAddDetails.Portfolio2 = trade.BuyBook;

            tradeAddDetails.Side = trade.SellBook.PortfolioId == trade.Portfolio.PortfolioId
                ? SideControl.Sell
                : SideControl.Buy;

            return tradeAddDetails;
        }

        private TradeAddDetails SetFxTradeDetails(
            Func<int, TryGetResult<FxTrade>> getFxTrade,
            int firstParentTradeId,
            TradeAddDetails tradeAddDetails)
        {
            TryGetResult<FxTrade> fxTradeResult = getFxTrade(firstParentTradeId);

            if (!fxTradeResult.HasValue)
            {
                return tradeAddDetails;
            }

            FxTrade trade = fxTradeResult.Value;
            Instrument instrument1 =
                _officialProductToInstrument.ConvertOfficialProductToInstrument(trade.Product.OfficialProduct);

            tradeAddDetails.SpecifiedAmount = trade.SpecifiedAmount;
            tradeAddDetails.FxExchangeRate = trade.Rate;
            tradeAddDetails.ForwardValueDate = trade.ValueDate;
            tradeAddDetails.FxSelectedInstrument = instrument1;
            tradeAddDetails.IsSpot = trade.ProductType == FxProductTypes.Spot;
            return tradeAddDetails;
        }

        private static TradeAddDetails SetTasTypeDetails(Product product, TradeAddDetails tradeAddDetails)
        {
            switch (product.TasType)
            {
                case TasType.Tas:
                {
                    tradeAddDetails.IsTasChecked = true;
                }
                break;
                case TasType.Mops:
                {
                    tradeAddDetails.IsMopsChecked = true;
                }
                break;
                case TasType.Mm:
                {
                    tradeAddDetails.IsMmChecked = true;
                }
                break;
            }

            return tradeAddDetails;
        }

        private static TradeAddDetails SetTimeSpreadLegPrices(
            List<TradeCapture> tradeCaptures,
            TradeCapture parentTrade,
            TradeAddDetails tradeAddDetails)
        {
            TradeCapture leg1Trade = tradeCaptures.Single(x => IsTimeSpreadFirstLeg(x, parentTrade));
            TradeCapture leg2Trade = tradeCaptures.Single(x => IsTimeSpreadSecondLeg(x, parentTrade));

            tradeAddDetails.StripDetail1.Leg1Price = leg1Trade.Price.Value;
            tradeAddDetails.StripDetail1.Leg2Price = leg2Trade.Price.Value;
            return tradeAddDetails;
        }

        private static bool IsTimeSpreadFirstLeg(TradeCapture tradeCapture, TradeCapture parentTrade)
        {
            return IsTimeSpreadLeg(tradeCapture) && (tradeCapture.Side == parentTrade.Side);
        }

        private static bool IsTimeSpreadLeg(TradeCapture tradeCapture)
        {
            return tradeCapture.IsParentTrade.HasValue && !tradeCapture.IsParentTrade.Value;
        }

        private static bool IsTimeSpreadSecondLeg(TradeCapture tradeCapture, TradeCapture parentTrade)
        {
            return IsTimeSpreadLeg(tradeCapture) && (tradeCapture.Side != parentTrade.Side);
        }

        private bool IsTimeSpread<T>(List<T> trades)
        {
            return trades.Count == 3;
        }

        private static TradeAddDetails SetTradeAddDetailsStripDetail2(
            TradeAddDetails tradeAddDetails,
            TradePieces trade,
            Instrument instrument,
            Product product,
            Func<string[], string> selectStripName)
        {
            tradeAddDetails.StripDetail2 = new StripDetail(tradeAddDetails);
            tradeAddDetails.StripDetail2.Instrument = instrument;
            tradeAddDetails.StripDetail2.Price = trade.Trade.Price.Value;

            tradeAddDetails.StripDetail2.Strip = Strip.FromTradeCapture(trade, selectStripName);
            tradeAddDetails.StripDetail2.Strip.SetTradeAddDetails(tradeAddDetails);

            tradeAddDetails.StripDetail2.Unit = product.Unit;
            tradeAddDetails.StripDetail2.Volume = Math.Abs(trade.Trade.Quantity.Value);

            return tradeAddDetails;
        }

        public TryGetResult<TradeAddDetails> ConvertTradeCaptureToTradeAddDetails(
            Func<int, TryGetResult<OfficialProduct>> getOfficialProductForSecDef,
            Func<int, TryGetResult<FxTrade>> getFxTrade,
            List<TradePieces> tradeCaptures,
            Action<TradeAddDetails, TradeCapture, IEnumerable<int>> setTradeDetailsFromParent)
        {
            List<TradePieces> parentTrades = GetParentTrades(tradeCaptures);

            if (parentTrades.Count == 0)
            {
                return new TryGetRef<TradeAddDetails>((TradeAddDetails)null);
            }

            TradePieces firstParentAndSecDef = parentTrades.First();
            TradeCapture firstParent = firstParentAndSecDef.Trade;
            int firstParentTradeId = firstParent.TradeId;
            TradeAddDetails tradeAddDetails = new TradeAddDetails();

            setTradeDetailsFromParent(
                tradeAddDetails,
                firstParent,
                tradeCaptures.Select(tradeAndSecDef => tradeAndSecDef.Trade.TradeId));

            tradeAddDetails.Broker = string.IsNullOrEmpty(firstParent.ExecutingFirm)
                ? "Internal"
                : firstParent.ExecutingFirm;
            tradeAddDetails.Exchange = string.IsNullOrEmpty(firstParent.ExchangeOverride)
                ? firstParent.Exchange
                : firstParent.ExchangeOverride;

            tradeAddDetails.TradeType = firstParent.TradeType == 1
                ? TradeTypeControl.Transfer
                : TradeTypeControl.Manual;

            // TODO: Can this be moved into the code setting up the parent trades collection above?
            List<TradePieces> oneBookTrades =
            tradeAddDetails.TradeType == TradeTypeControl.Transfer ? GetOneBookTrades(parentTrades) : parentTrades;

            if (tradeAddDetails.TradeType == TradeTypeControl.Manual)
            {
                // TODO: Why does this still use the same first parent trade even though the parent trades collection
                // has changed.
                tradeAddDetails.Side = firstParent.Side.Equals("buy", StringComparison.InvariantCultureIgnoreCase)
                    ? SideControl.Buy
                    : SideControl.Sell;
            }

            tradeAddDetails.StripTypeControl = GetStripTypeControl(oneBookTrades);

            TradeCapture firstOneBookTrade = oneBookTrades[0].Trade;
            SecurityDefinition firstOneBookSecDef = oneBookTrades[0].Security.SecurityDef;
            TryGetResult<OfficialProduct> firstParentOffProd =
                getOfficialProductForSecDef(firstOneBookSecDef.SecurityDefinitionId);
            Product product = oneBookTrades[0].Security.Product;

            if (!firstParentOffProd.HasValue)
            {
                return new TryGetRef<TradeAddDetails>((TradeAddDetails)null);
            }

            Instrument firstOneBookInstr =
                _officialProductToInstrument.ConvertOfficialProductToInstrument(firstParentOffProd.Value);

            tradeAddDetails.StripDetail1 = new StripDetail(tradeAddDetails);
            tradeAddDetails.StripDetail1.Instrument = firstOneBookInstr;

            if (tradeAddDetails.StripTypeControl != StripTypeControl.FX)
            {
                tradeAddDetails.StripDetail1.Price = firstOneBookTrade.Price.Value;
                tradeAddDetails.StripDetail1.Strip = Strip.FromTradeCapture(
                    oneBookTrades[0],
                    Strip.DefaultStripNameSelector(true));
                tradeAddDetails.StripDetail1.Strip.SetTradeAddDetails(tradeAddDetails);
                tradeAddDetails.StripDetail1.Unit = product.Unit;
                tradeAddDetails.StripDetail1.Volume = Math.Abs(firstOneBookTrade.Quantity.Value);

                if (tradeAddDetails.StripDetail1.Instrument.IsCalcPnlFromLegs)
                {
                    SetLegPrices(tradeCaptures, tradeAddDetails, product);
                }
            }

            SetTasTypeDetails(product, tradeAddDetails);

            if (firstOneBookSecDef.StripName != null && firstOneBookSecDef.StripName.IndexOf('/') > 0)
            {
                // Use the second leg strip name
                SetTradeAddDetailsStripDetail2(
                    tradeAddDetails,
                    oneBookTrades[0],
                    firstOneBookInstr,
                    product,
                    (stripParts) => stripParts[1]);

                // TODO: The check that was here has been moved into a method called IsTimeSpread based on the 
                // assumption that the check for 3 trades is a check for a time spread.  This must be confirmed or the
                // name of the new method changed.
                if (IsTimeSpread(tradeCaptures))
                {
                    SetTimeSpreadLegPrices(
                        tradeCaptures.Select(tradeAndSecDef => tradeAndSecDef.Trade).ToList(),
                        firstOneBookTrade,
                        tradeAddDetails);
                }
            }

            if (oneBookTrades.Count > 1)
            {
                SecurityDefinition secondOneBookSecDef = oneBookTrades[1].Security.SecurityDef;
                TryGetResult<OfficialProduct> secondOneBookOffProd =
                    getOfficialProductForSecDef(secondOneBookSecDef.SecurityDefinitionId);

                if (!secondOneBookOffProd.HasValue)
                {
                    return new TryGetRef<TradeAddDetails>((TradeAddDetails)null);
                }

                Product secondOneBookProduct = oneBookTrades[1].Security.Product;

                // TODO: Why does this use the first strip name?
                SetTradeAddDetailsStripDetail2(
                    tradeAddDetails,
                    oneBookTrades[1],
                    _officialProductToInstrument.ConvertOfficialProductToInstrument(secondOneBookOffProd.Value),
                    secondOneBookProduct,
                    (stripParts) => stripParts[0]);
            }

            if (tradeAddDetails.StripTypeControl == StripTypeControl.FX)
            {
                SetFxTradeDetails(getFxTrade, firstParentTradeId, tradeAddDetails);
            }

            if (tradeAddDetails.TradeType == TradeTypeControl.Transfer)
            {
                SetTransferTradePortfolio(tradeAddDetails, firstOneBookTrade);
            }
            else
            {
                tradeAddDetails.Portfolio1 = firstOneBookTrade.Portfolio;
            }

            if (WillProduceTwoParentTrades(tradeAddDetails))
            {
                SetTimeSpreadAddDetailsPrices(tradeAddDetails);
            }

            return new TryGetRef<TradeAddDetails>(tradeAddDetails);
        }

        private static bool WillProduceTwoParentTrades(TradeAddDetails tradeAddDetails)
        {
            return (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                   && (IsSpreadWithBalmoOrExactlyOneQOrCalStrip(tradeAddDetails));
        }

        private static bool IsSpreadWithBalmoOrExactlyOneQOrCalStrip(TradeAddDetails tradeAddDetails)
        {
            return tradeAddDetails.SpreadHasABalmoStrip() || tradeAddDetails.IsSpreadWithOneQOrCalStrip();
        }

        private static List<TradePieces> GetParentTrades(List<TradePieces> trades)
        {
            List<TradePieces> parentTrades =
                trades.Where(tradeAndSecDef => tradeAndSecDef.Trade.IsParentTrade.True()).ToList();

            if (parentTrades.Any())
            {
                TradePieces firstParentTrade = parentTrades.First();

                if (firstParentTrade.Trade.TradeType == null)
                {
                    // if it's an exchange trade we would have the whole time spread trades here 
                    // and we need them only for cancel trades purpose which we already record
                    // hence we clear trades from the list so that they dont interfer in the trade convert logic
                    parentTrades = new List<TradePieces>() { firstParentTrade };
                }
            }

            return parentTrades;
        }

        private List<TradePieces> GetOneBookTrades(
            List<TradePieces> tradeCaptures)
        {
            Func<TradeCapture, bool> mustAddTrade = GetOneBookTradeFilter(tradeCaptures);

            return tradeCaptures.Where(trade => mustAddTrade(trade.Trade)).ToList();
        }

        private static Func<TradeCapture, bool> GetOneBookTradeFilter(List<TradePieces> trades)
        {
            Func<TradeCapture, bool> mustAddTrade;

            switch (trades.Count)
            {
                case 4:
                {
                    mustAddTrade = (trade) => trade.IsTransferSell.True();
                }
                break;

                case 2:
                {
                    mustAddTrade = (trade) => TradeSide.IsBuy(trade.Side);
                }
                break;

                default:
                {
                    mustAddTrade = (trade) => false;
                }
                break;
            }
            return mustAddTrade;
        }

        private StripTypeControl GetStripTypeControl(List<TradePieces> trades)
        {
            switch (trades.Count)
            {
                case 1:
                {
                    return GetStripTypeForSimpleTrade(trades);
                }

                case 2:
                {
                    return GetStripTypeForComplexTrade(trades);
                }

                default:
                {
                    return StripTypeControl.Flat;
                }
            }
        }

        private static StripTypeControl GetStripTypeForSimpleTrade(List<TradePieces> trades)
        {
            TradePieces firstTrade = trades[0];

            if (firstTrade.Trade.SecurityDefinitionId == SecurityDefinitionsManager.FxSecDefId)
            {
                return StripTypeControl.FX;
            }

            if (firstTrade.Security.Product.Type == ProductType.DailySwap)
            {
                return StripTypeControl.DailySwap;
            }

            if ((firstTrade.Security.Product.Type.IsDailyOrWeeklyDiff()))
            {
                return StripTypeControl.DailyDiff;
            }

            if (firstTrade.Security.SecurityDef.StripName.IndexOf('/') > 0)
            {
                return StripTypeControl.Spread;
            }

            Tuple<DateTime, ProductDateType> liveTradeDate = StripHelper.ParseStripDate(
                firstTrade.Security.SecurityDef.StripName,
                firstTrade.Trade.TradeStartDate ?? DateTime.MinValue,
                firstTrade.Trade.UtcTransactTime);

            if (liveTradeDate.Item2 == ProductDateType.Custom)
            {
                return StripTypeControl.CustomMonthly;
            }

            return StripTypeControl.Flat;
        }

        private static StripTypeControl GetStripTypeForComplexTrade(List<TradePieces> trades)
        {
            if ((trades[0].Security.Product.Type == ProductType.Futures)
                || (trades[1].Security.Product.Type == ProductType.Futures))
            {
                return StripTypeControl.FutureVsSwap;
            }

            return StripTypeControl.Spread;
        }

        private void SetLegPrices(
            List<TradePieces> trades,
            TradeAddDetails tradeAddDetails,
            Product product)
        {
            StripDetail stripDetail = tradeAddDetails.StripDetail1;
            IEnumerable<TradePieces> leg1ProductTrades =
                GetTradesForProduct(trades, product.ComplexProduct.ChildProduct1_Id.Value);
            IEnumerable<TradePieces> leg2ProductTrades =
                GetTradesForProduct(trades, product.ComplexProduct.ChildProduct2_Id.Value);

            if (tradeAddDetails.TradeType == TradeTypeControl.Transfer)
            {
                leg1ProductTrades = leg1ProductTrades.Where(trade => trade.Trade.IsTransferSell.True());
                leg2ProductTrades = leg2ProductTrades.Where(trade => trade.Trade.IsTransferSell.True());
            }

            stripDetail.Leg1Price = SumTradePrices(leg1ProductTrades);
            stripDetail.Leg2Price = SumTradePrices(leg2ProductTrades);
        }

        private static IEnumerable<TradePieces> GetTradesForProduct(List<TradePieces> trades, int productId)
        {
            return trades.Where(trade => trade.Security.Product.ProductId == productId);
        }

        private static decimal SumTradePrices(IEnumerable<TradePieces> legTrades)
        {
            decimal totalCost = legTrades.Sum(trade => trade.Trade.Price.Value * trade.Trade.Quantity.Value);
            decimal totalVolume = legTrades.Sum(trade => trade.Trade.Quantity.Value);

            return totalCost / totalVolume;
        }
    }
}