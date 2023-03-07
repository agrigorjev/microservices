using Mandara.Date.Time;
using Mandara.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Transactions;

namespace Mandara.Business.Managers
{
    public class PerformanceLogManager : IPerformanceLogManager, IDisposable
    {
        private struct PerformanceEntry
        {
            public DateTime EntryTime { get; }
            public List<int> TradeIds { get; }
            public Action<PerformanceLogMessage, DateTime> LogAction { get; }

            public PerformanceEntry(DateTime entryTime, List<int> tradeIds, Action<PerformanceLogMessage, DateTime> logAct)
            {
                EntryTime = entryTime;
                TradeIds = tradeIds;
                LogAction = logAct;
            }
        }

        private readonly ConcurrentQueue<PerformanceEntry> _performanceEntries = new ConcurrentQueue<PerformanceEntry>();
        private Timer _queueReadTimer;
        private const int QueueReadTimeout = 1000;
        private const int QueueReadRepeatPeriod = Timeout.Infinite;

        public PerformanceLogManager()
        {
            StartUp();
        }

        private void StartUp()
        {
            if ((ConfigurationManager.AppSettings["PerformanceLog_Enabled"] ?? "false").Equals(
                "false",
                StringComparison.InvariantCultureIgnoreCase))
            {
                // skip performance logging if we have a switch in app.config
                return;
            }

            _queueReadTimer = new Timer(
                SavePerformanceDataFromQueue,
                _performanceEntries,
                QueueReadTimeout,
                QueueReadRepeatPeriod);
        }

        private void SavePerformanceDataFromQueue(object performanceEntriesQueue)
        {
            ConcurrentQueue<PerformanceEntry> performanceEntries =
                performanceEntriesQueue as ConcurrentQueue<PerformanceEntry>;
            PerformanceEntry entry;

            while (performanceEntries.TryDequeue(out entry))
            {
                SavePerformanceLogEntry(entry);
            }

            StartQueueReadTimer();
        }

        private static void SavePerformanceLogEntry(PerformanceEntry entry)
        {
            List<int> tradeIds = entry.TradeIds;

            using (TransactionScope trx = CreateTransactionScope())
            {
                using (MandaraEntities cxt = new MandaraEntities(
                    MandaraEntities.DefaultConnStrName,
                    nameof(PerformanceLogManager)))
                {
                    foreach (int tradeId in tradeIds)
                    {
                        PerformanceLogMessage perfLogMsg = new PerformanceLogMessage { TradeCaptureId = tradeId };

                        entry.LogAction(perfLogMsg, entry.EntryTime);
                        cxt.PerformanceLogMessages.Add(perfLogMsg);
                    }

                    cxt.SaveChanges();
                }

                trx.Complete();
            }
        }

        private static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });
        }

        private void StartQueueReadTimer()
        {
            _queueReadTimer.Change(1000, Timeout.Infinite);
        }

        public void SaveTradesStats(
            List<int> tradeIds,
            Action<PerformanceLogMessage, DateTime> setPerformanceLogMessageAction)
        {
            if (tradeIds.Count == 0)
            {
                return;
            }

            DateTime now = SystemTime.Now();

            _performanceEntries.Enqueue(new PerformanceEntry(now, tradeIds, setPerformanceLogMessageAction));
        }

        public void Dispose()
        {
            WaitHandle timerDisposeHandle = new ManualResetEvent(false);

            _queueReadTimer.Dispose(timerDisposeHandle);
            timerDisposeHandle.WaitOne(1000);
        }
    }
}