using System.Collections.Generic;
using System.Linq;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.Exposures.SwapCrossAutomation
{
    public class TransferTradesResponseMessage : MessageBase
    {
        public Dictionary<string, (int product, List<List<TradeCapture>> trades)> Trades { get; set; }

        public List<SuggestedTrade> GetTrades()
        {
            return Trades?.SelectMany(
                             tradesForExposure => SuggestedTradesFromExposureTrades(
                                 tradesForExposure.Key,
                                 tradesForExposure.Value.product,
                                 tradesForExposure.Value.trades))
                         .ToList()
                   ?? new List<SuggestedTrade>();
        }

        private IEnumerable<SuggestedTrade> SuggestedTradesFromExposureTrades(
            string exposedTo,
            int exposureProduct,
            IEnumerable<List<TradeCapture>> expTrades)
        {
            return expTrades.SelectMany(
                (trades, idx) => ExtractSuggestedTrades(trades, exposedTo, exposureProduct, idx));
        }

        IEnumerable<SuggestedTrade> ExtractSuggestedTrades(
            List<TradeCapture> trades,
            string exposedTo,
            int exposureProduct,
            int exposureOption)
        {
            return trades.Select(trade => new SuggestedTrade(exposureOption, exposedTo, exposureProduct, trade));
        }
    }

    public class SuggestedTrade
    {
        public int OptionId { get; }
        public string ExposedTo { get; }
        public string Exposure => $"{ExposedTo} (opt {OptionId})";
        public int ExposureProduct { get; }
        public TradeCapture Trade { get; }

        public SuggestedTrade(int optionId, string exposedTo, int exposureProduct, TradeCapture trade)
        {
            OptionId = optionId;
            ExposedTo = exposedTo;
            ExposureProduct = exposureProduct;
            Trade = trade;
        }
    }
}