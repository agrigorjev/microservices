using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mandara.Entities.Trades
{
    public abstract class TradesDataPackBase<Trade, TradeChg> : IRetriable
        where Trade : class where TradeChg : TradeChange
    {
        public const int DefaultMaxDisplayableIdentifiers = 11;
        public const string IdStringGap = ",..., ";
        public List<Trade> Trades { get; private set; }
        public List<TradeChg> TradeChanges { get; private set; }
        public int RetryCounter { get; private set; }

        protected TradesDataPackBase(List<Trade> trades, List<TradeChg> tradeChanges, int retryCounter)
        {
            Trades = trades ?? new List<Trade>();
            TradeChanges = tradeChanges ?? new List<TradeChg>();
            RetryCounter = retryCounter;
        }

        public void IncrementRetryCounter()
        {
            RetryCounter += 1;
        }

        public bool IsEmpty()
        {
            return Trades.Count == 0 && TradeChanges.Count == 0;
        }

        protected string ToString(string format, int maxDelayedIds)
        {
            string tradesIds = GetIdsString(Trades, GetTradeId, maxDelayedIds - 1);
            string tradeChangesIds = GetIdsString(TradeChanges, GetTradeChangeId, maxDelayedIds - 1);

            return String.Format(format, tradesIds, tradeChangesIds, RetryCounter);
        }

        protected static string GetIdsString<T>(
            List<T> tradesOrChanges,
            Func<T, int> getId,
            int boundaryCollectionLength)
        {
            StringBuilder identifierString =
                new StringBuilder(String.Join(", ", tradesOrChanges.Take(boundaryCollectionLength).Select(getId)));

            if (tradesOrChanges.Count > boundaryCollectionLength)
            {
                identifierString.AppendFormat("{0}{1}", IdStringGap, getId(tradesOrChanges.Last()));
            }

            return identifierString.ToString();
        }

        protected abstract int GetTradeId(Trade trade);
        protected abstract int GetTradeChangeId(TradeChg trade);
    }
}
