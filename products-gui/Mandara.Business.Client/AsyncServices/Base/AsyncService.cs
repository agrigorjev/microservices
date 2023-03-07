using Mandara.Entities.ErrorReporting;
using Ninject.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading;

namespace Mandara.Business.AsyncServices.Base
{
    public abstract class AsyncService : IDisposable
    {
        protected readonly ILogger _log;
        public TimeSpan SleepTime { get; set; }
        private BackgroundWorker Worker { get; set; }
        private readonly CancellationTokenSource _source;

        protected AsyncService(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
            _source = new CancellationTokenSource();
            SleepTime = TimeSpan.FromSeconds(10);
            Worker = CreateWorker();
        }

        private BackgroundWorker CreateWorker()
        {
            BackgroundWorker result = new BackgroundWorker();
            result.WorkerSupportsCancellation = true;
            result.DoWork += Worker_DoWork;
            result.RunWorkerCompleted += LogErrorAndRestartWorker_RunWorkerCompleted;

            return result;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker == null)
            {
                return;
            }

            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                DoWork();

                if (SleepTime != TimeSpan.Zero)
                {
                    bool cancelled = _source.Token.WaitHandle.WaitOne(SleepTime);
                    if (cancelled)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        protected abstract void DoWork();

        internal void LogErrorAndRestartWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker == null)
                return;
            
            if (e.Cancelled || _source.IsCancellationRequested)
            {
                _source.Dispose();
                return;
            }
            if (e.Error != null)
            {
                string typeName = GetType().Name;
                string errorMsg = typeName + " encountered an error.";
                Error error = new Error(
                    typeName,
                    ErrorType.Exception,
                    errorMsg,
                    null,
                    e.Error.ToString(),
                    ErrorLevel.Critical);

                _log.Error(e.Error, errorMsg);
                ErrorReportingHelper.GlobalQueue.Enqueue(error);

                worker.RunWorkerAsync();
            }
        }

        public void Start()
        {
            if (!Worker.IsBusy)
                Worker.RunWorkerAsync();

            OnStarted();
        }

        protected virtual void OnStarted() { }

        public virtual void Stop()
        {
            _source.Cancel();
            Worker.CancelAsync();

            OnStopped();
        }

        protected virtual void OnStopped() { }

        public bool IsRunning { get { return Worker.IsBusy; } }

        public void Dispose()
        {
            if (Worker != null)
            {
                Worker.Dispose();
                Worker = null;
            }

        }

        public delegate void CancellableAction(CancellationToken token);

        internal class ActionAsyncService : AsyncService
        {
            private readonly Action _action;

            public ActionAsyncService(Action action, ILogger log)
                : base(log)
            {
                _action = action;
            }

            protected override void DoWork()
            {
                if (Worker.CancellationPending)
                {
                    return;
                }

                _action();
            }
        }

        internal class CancelActionAsyncService : AsyncService
        {
            private readonly CancellableAction _action;

            public CancelActionAsyncService(CancellableAction action, ILogger log)
                : base(log)
            {
                _action = action;
            }

            protected override void DoWork()
            {
                if (Worker.CancellationPending)
                {
                    return;
                }

                _action(_source.Token);
            }
        }
    }
}

