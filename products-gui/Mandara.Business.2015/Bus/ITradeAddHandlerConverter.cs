using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.TradeAdd;
using Mandara.Entities;
using Mandara.Entities.EntityPieces;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus
{
    public interface ITradeAddHandlerConverter
    {
        TradeDataForTradeAdd GetTradeCaptures(TradeAddDetails tradeAddDetails, bool isMasterToolMode);

        Dictionary<Instrument, OfficialProduct> GetInstrumentsProductsMap(bool isMasterToolMode);

        Action<TradeAddDetails, TradeCapture, IEnumerable<int>> GetActionSetTradeDetailsFromParent(bool isDuplicateMode);

        TryGetResult<TradeAddDetails> ConvertTradeCaptureToTradeAddDetails(
            Func<int, TryGetResult<OfficialProduct>> getOfficialProductForSecDef,
            Func<int, TryGetResult<FxTrade>> getFxTrade,
            List<TradeCapture> tradeCaptures,
            Action<TradeAddDetails, TradeCapture, IEnumerable<int>> setTradeDetailsFromParent);

        TryGetResult<TradeAddDetails> ConvertTradeCaptureToTradeAddDetails(
            Func<int, TryGetResult<OfficialProduct>> getOfficialProductForSecDef,
            Func<int, TryGetResult<FxTrade>> getFxTrade,
            List<TradePieces> tradeCaptures,
            Action<TradeAddDetails, TradeCapture, IEnumerable<int>> setTradeDetailsFromParent);
    }
}
