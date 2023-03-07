using com.latencybusters.lbm;
using NLog;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;

namespace Mandara.IRM.Server.Bus
{
    public class TempTopicQueue
    {
        public readonly Logger _log = LogManager.GetCurrentClassLogger();

        private int _numQueueLength = 20;
        private int _numQueueMin = 10;

        private readonly LBMContext _lbmContext;
        private readonly string _topicNameInvariantPart;
        private readonly ConcurrentDictionary<Guid, LBMSource> _dictSources = new ConcurrentDictionary<Guid, LBMSource>();
        private readonly ConcurrentQueue<Guid> _queueIds = new ConcurrentQueue<Guid>();
        private BackgroundWorker _fillWorker;

        public TempTopicQueue(LBMContext lbmContext, string topicNameInvariantPart)
        {
            _lbmContext = lbmContext;
            _topicNameInvariantPart = topicNameInvariantPart;

            _fillWorker = new BackgroundWorker();
            _fillWorker.WorkerSupportsCancellation = true;
            _fillWorker.DoWork += FillWorker_DoWork;
            _fillWorker.RunWorkerCompleted += LogError_RunWorkerCompleted;
            _fillWorker.RunWorkerAsync();
        }

        void LogError_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            if (worker == null)
                return;

            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                _log.Error(e.Error, "IRM Server encountered an error.");
            }
        }

        private void FillWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker == null)
                return;

            while (true)
            {
                if (worker.CancellationPending)
                    return;

                if (_queueIds.Count <= _numQueueMin)
                {
                    while (_queueIds.Count <= _numQueueLength)
                    {
                        Guid id = Guid.NewGuid();
                        string topicName = string.Format("{0}/{1}", _topicNameInvariantPart, id);

                        LBMTopic lbmTopic = new LBMTopic(_lbmContext, topicName, new LBMSourceAttributes());
                        LBMSource lbmSource = new LBMSource(_lbmContext, lbmTopic);

                        _dictSources.TryAdd(id, lbmSource);
                        _queueIds.Enqueue(id);

                        _log.Info("Created new temporary source [" + topicName + "]");
                    }
                }

                Thread.Sleep(50);
            }
        }

        public Guid DequeueId()
        {
            Guid id;

            if (!_queueIds.TryDequeue(out id))
                id = Guid.Empty;

            return id;
        }

        public LBMSource GetLbmSource(Guid id)
        {
            LBMSource lbmSource;

            if (!_dictSources.TryRemove(id, out lbmSource))
                lbmSource = null;

            return lbmSource;
        }

        public void Clear()
        {
            try
            {
                if (_fillWorker != null)
                    _fillWorker.CancelAsync();

                foreach (Guid key in _dictSources.Keys)
                {
                    LBMSource lbmSource;

                    if (_dictSources.TryRemove(key, out lbmSource))
                    {
                        if (lbmSource != null && !lbmSource.isClosed())
                            lbmSource.close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}