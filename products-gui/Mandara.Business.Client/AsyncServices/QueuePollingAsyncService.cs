using Mandara.Business.Bus;
using Mandara.Business.Bus.Commands.Base;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Threading;

namespace Mandara.Business.AsyncServices
{
    public abstract class QueuePollingAsyncService<T> : ClientAsyncService where T : class, IMessage
    {
        private readonly SequenceQueue<T> _queue;
        private readonly TimeSpan DefaultWaitTime = TimeSpan.FromSeconds(1);

        public QueuePollingAsyncService(CommandManager commandManager, BusClient busClient, ILogger log, SequenceQueue<T> queue) : base(commandManager, busClient, log)
        {
            _queue = queue;
            SleepTime = TimeSpan.Zero;
        }

        protected virtual bool IsQueueReady()
        {
            return true;
        }

        protected void SleepForQueue()
        {
            Thread.Sleep(50);
        }

        protected TryGetResult<T> PollForNextElement()
        {
            return _queue.TryDequeue(DefaultWaitTime);
        }

    }
}
