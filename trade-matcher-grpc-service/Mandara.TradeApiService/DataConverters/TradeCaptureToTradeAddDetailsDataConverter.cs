using Mandara.GrpcDefinitions.Extensions;
using Mandara.TradeApiService.Data;
using Mandara.TradeApiService.Contracts;
using Mandara.TradeApiService.GrpcDefinitions;
using Mandara.TradeApiService;

using System.Diagnostics.Metrics;
using Google.Protobuf.WellKnownTypes;

namespace Mandara.TradeApiService.DataConverters
{
    public class TradeCaptureToTradeAddDetailsDataConverter : IDataConverter<TradeCapture, TradeAddDetailsGrpc>
    {
        private readonly DataConverters.OfficialProductToInstrumentDataConverter _officalProductToInstrument = new DataConverters.OfficialProductToInstrumentDataConverter();
        private readonly DataConverters.UnitDataConverter _unitConverter = new DataConverters.UnitDataConverter();
        private static readonly DataConverters.PortfolioDataConverter _portfolioConverter = new DataConverters.PortfolioDataConverter();

        private static readonly int FxSecDefId = -1;

        public TradeAddDetailsGrpc? ConvertM(Func<int, OfficialProduct?> getOfficialProductForSecDef, 
            Func<int, FxTrade?> getFxTrade, 
            List<TradeCapture> tradeCaptures, 
            Action<TradeAddDetailsGrpc, TradeCapture, IEnumerable<int>> setTradeDetailsFromParent)
        {
            List<TradePieces> tradePieces = tradeCaptures.Select(trade => new TradePieces(trade)).ToList();
            List < TradePieces > parentTrades = GetParentTrades(tradePieces).ToList();

            if (parentTrades.Count == 0)
            {
                return null;
            }

            TradePieces firstParentAndSecDef = parentTrades.First();
            TradeCapture firstParent = firstParentAndSecDef.Trade;
            int firstParentTradeId = firstParent.TradeId;
            TradeAddDetailsGrpc tradeAddDetails = new TradeAddDetailsGrpc();

            setTradeDetailsFromParent(
                tradeAddDetails,
                firstParent,
                tradeCaptures.Select(tradeAndSecDef => tradeAndSecDef.TradeId));

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
            OfficialProduct firstParentOffProd =
                getOfficialProductForSecDef(firstOneBookSecDef.SecurityDefinitionId);
            Product product = oneBookTrades[0].Security.Product;

            if (firstParentOffProd == null)
            {
                return null;
            }

            InstrumentGrpc firstOneBookInstr =
                _officalProductToInstrument.Convert(firstParentOffProd);

            tradeAddDetails.StripDetail1 = new StripDetailGrpc();
            tradeAddDetails.StripDetail1.Instrument = firstOneBookInstr;

            if (tradeAddDetails.StripTypeControl != StripTypeControl.Fx)
            {
                tradeAddDetails.StripDetail1.Price = firstOneBookTrade.Price.Value;
                tradeAddDetails.StripDetail1.Strip = StripGrpc.FromTradeCapture(
                    oneBookTrades[0],
                    StripGrpc.DefaultStripNameSelector(true));
                //TODO:Do we need it?
                //tradeAddDetails.StripDetail1.Strip.SetTradeAddDetails(tradeAddDetails);
                tradeAddDetails.StripDetail1.Unit = _unitConverter.Convert(product.Unit);
                tradeAddDetails.StripDetail1.Volume = Math.Abs(firstOneBookTrade.Quantity.Value);

                if (tradeAddDetails.StripDetail1.Instrument.IsCalcPnlFromLegs)
                {
                    SetLegPrices(tradePieces, tradeAddDetails, product);
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
                    _unitConverter.Convert(product.Unit),
                    (stripParts) => stripParts[1]);

                // TODO: The check that was here has been moved into a method called IsTimeSpread based on the 
                // assumption that the check for 3 trades is a check for a time spread.  This must be confirmed or the
                // name of the new method changed.
                if (IsTimeSpread(tradeCaptures))
                {
                    SetTimeSpreadLegPrices(
                        tradePieces.Select(tradeAndSecDef => tradeAndSecDef.Trade).ToList(),
                        firstOneBookTrade,
                        tradeAddDetails);
                }
            }

            if (oneBookTrades.Count > 1)
            {
                SecurityDefinition secondOneBookSecDef = oneBookTrades[1].Security.SecurityDef;
                OfficialProduct secondOneBookOffProd =
                    getOfficialProductForSecDef(secondOneBookSecDef.SecurityDefinitionId);

                if (secondOneBookOffProd == null)
                {
                    return null;
                }

                Product secondOneBookProduct = oneBookTrades[1].Security.Product;

                // TODO: Why does this use the first strip name?
                SetTradeAddDetailsStripDetail2(
                    tradeAddDetails,
                    oneBookTrades[1],
                    _officalProductToInstrument.Convert(secondOneBookOffProd),
                    _unitConverter.Convert(secondOneBookProduct.Unit),
                    (stripParts) => stripParts[0]);
            }

            if (tradeAddDetails.StripTypeControl == StripTypeControl.Fx)
            {
                SetFxTradeDetails(getFxTrade, firstParentTradeId, tradeAddDetails);
            }

            if (tradeAddDetails.TradeType == TradeTypeControl.Transfer)
            {
                SetTransferTradePortfolio(tradeAddDetails, firstOneBookTrade);
            }
            else
            {
                tradeAddDetails.Portfolio1 = _portfolioConverter.Convert(firstOneBookTrade.Portfolio);
            }

            if (WillProduceTwoParentTrades(tradeAddDetails))
            {
                SetTimeSpreadAddDetailsPrices(tradeAddDetails);
            }

            return tradeAddDetails;
        }

        private bool IsTimeSpread<T>(List<T> trades)
        {
            return trades.Count == 3;
        }

        private static TradeAddDetailsGrpc SetTimeSpreadLegPrices(
            List<TradeCapture> tradeCaptures,
            TradeCapture parentTrade,
            TradeAddDetailsGrpc tradeAddDetails)
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

        public Action<TradeAddDetailsGrpc, TradeCapture, IEnumerable<int>> GetActionSetTradeDetailsFromParent(
                    bool isDuplicateMode)
        {
            if (!isDuplicateMode)
            {
                return SetNonDuplicateModeTradeAddDetails;
            }

            return (tradeDetails, parentTrade, trades) => { };
        }

        private static void SetNonDuplicateModeTradeAddDetails(
            TradeAddDetailsGrpc tradeAddDetails,
            TradeCapture firstParentTrade,
            IEnumerable<int> tradeIds)
        {
            tradeAddDetails.TradeCaptureIds.AddRange(tradeIds.ToList());
            tradeAddDetails.GroupId = firstParentTrade.GroupId;
            tradeAddDetails.TradeDate = firstParentTrade.TradeDate.toProtoTimestamp();
            tradeAddDetails.TimestampUtc = firstParentTrade.TimeStamp.toProtoTimestamp();
            tradeAddDetails.TransactTimeUtc = firstParentTrade.TransactTime.toProtoTimestamp();
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

        public TradeAddDetailsGrpc Convert(TradeCapture data)
        {
            throw new NotImplementedException();
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

            if (firstTrade.Trade.SecurityDefinitionId == FxSecDefId)
            {
                return StripTypeControl.Fx;
            }

            if (firstTrade.Security.Product.Type == ProductType.DailySwap)
            {
                return StripTypeControl.Dailyswap;
            }

            if ((firstTrade.Security.Product.Type.IsDailyOrWeeklyDiff()))
            {
                return StripTypeControl.Dailtydiff;
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
                return StripTypeControl.Custommonthly;
            }

            return StripTypeControl.Flat;
        }

        private static StripTypeControl GetStripTypeForComplexTrade(List<TradePieces> trades)
        {
            if ((trades[0].Security.Product.Type == ProductType.Futures)
                || (trades[1].Security.Product.Type == ProductType.Futures))
            {
                return StripTypeControl.Futurevsswap;
            }

            return StripTypeControl.Spread;
        }

        private void SetLegPrices(
            List<TradePieces> trades,
            TradeAddDetailsGrpc tradeAddDetails,
            Product product)
        {
            StripDetailGrpc stripDetail = tradeAddDetails.StripDetail1;
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

        private static TradeAddDetailsGrpc SetTasTypeDetails(Product product, TradeAddDetailsGrpc tradeAddDetails)
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

        private static TradeAddDetailsGrpc SetTradeAddDetailsStripDetail2(
            TradeAddDetailsGrpc tradeAddDetails,
            TradePieces trade,
            InstrumentGrpc instrument,
            UnitGrpc unit,
            Func<string[], string> selectStripName)
        {
            tradeAddDetails.StripDetail2 = new StripDetailGrpc();
            tradeAddDetails.StripDetail2.Instrument = instrument;
            tradeAddDetails.StripDetail2.Price = trade.Trade.Price.Value;

            tradeAddDetails.StripDetail2.Strip = StripGrpc.FromTradeCapture(trade, selectStripName);
            //tradeAddDetails.StripDetail2.Strip.SetTradeAddDetails(tradeAddDetails);

            tradeAddDetails.StripDetail2.Unit = unit;
            tradeAddDetails.StripDetail2.Volume = Math.Abs(trade.Trade.Quantity.Value);

            return tradeAddDetails;
        }

        private TradeAddDetailsGrpc SetFxTradeDetails(
            Func<int, FxTrade?> getFxTrade,
            int firstParentTradeId,
            TradeAddDetailsGrpc tradeAddDetails)
        {
            FxTrade? fxTradeResult = getFxTrade(firstParentTradeId);

            if (fxTradeResult == null)
            {
                return tradeAddDetails;
            }

            FxTrade trade = fxTradeResult;
            InstrumentGrpc instrument1 =
                _officalProductToInstrument.Convert(trade.Product.OfficialProduct);

            tradeAddDetails.SpecifiedAmount = trade.SpecifiedAmount;
            tradeAddDetails.FxExchangeRate = trade.Rate;
            tradeAddDetails.ForwardValueDate = trade.ValueDate.toProtoTimestamp();
            tradeAddDetails.FxSelectedInstrument = instrument1;
            tradeAddDetails.IsSpot = trade.ProductType == FxProductTypes.Spot;
            return tradeAddDetails;
        }

        private static TradeAddDetailsGrpc SetTransferTradePortfolio(TradeAddDetailsGrpc tradeAddDetails, TradeCapture trade)
        {
            tradeAddDetails.Portfolio1 = _portfolioConverter.Convert(trade.SellBook);
            tradeAddDetails.Portfolio2 = _portfolioConverter.Convert(trade.BuyBook);

            tradeAddDetails.Side = trade.SellBook.PortfolioId == trade.Portfolio.PortfolioId
                ? SideControl.Sell
                : SideControl.Buy;

            return tradeAddDetails;
        }

        private static bool WillProduceTwoParentTrades(TradeAddDetailsGrpc tradeAddDetails)
        {
            return (tradeAddDetails.StripTypeControl == StripTypeControl.Spread)
                   && (IsSpreadWithBalmoOrExactlyOneQOrCalStrip(tradeAddDetails));
        }

        private static bool IsSpreadWithBalmoOrExactlyOneQOrCalStrip(TradeAddDetailsGrpc tradeAddDetails)
        {
            return tradeAddDetails.SpreadHasABalmoStrip() || tradeAddDetails.IsSpreadWithOneQOrCalStrip();
        }

        private static TradeAddDetailsGrpc SetTimeSpreadAddDetailsPrices(TradeAddDetailsGrpc tradeAddDetails)
        {
            tradeAddDetails.StripDetail1.Leg1Price = tradeAddDetails.StripDetail1.Price;
            tradeAddDetails.StripDetail1.Leg2Price = tradeAddDetails.StripDetail2.Price;
            tradeAddDetails.StripDetail1.Price = tradeAddDetails.StripDetail1.Leg1Price
                                                 - tradeAddDetails.StripDetail1.Leg2Price;
            return tradeAddDetails;
        }
    }
}
