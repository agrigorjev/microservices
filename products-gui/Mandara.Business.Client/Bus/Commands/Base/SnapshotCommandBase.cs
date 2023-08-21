using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mandara.Business.Bus.Commands.Base
{
    using Mandara.Business.Bus.Messages;

    public class SnapshotCommandBase : BusCommandBase
    {
        public SnapshotMessageBase Message { get; set; }
        public LBMSource LbmTemporarySource { get; set; }
        public string TemporaryTopicName { get { return DeliveryContext.TempTopicName; } }
        public Logger Log { get; private set; }
        public SnapshotDeliveryContext DeliveryContext { get; set; }

        public SnapshotCommandBase()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

        public virtual void DoExecute()
        {

        }

        private bool _supportsGuaranteedDelivery = false;

        public virtual bool SupportsGuaranteedDelivery
        {
            get { return _supportsGuaranteedDelivery; }
            set { _supportsGuaranteedDelivery = value; }
        }

        public override void Execute()
        {
            DoExecute();

            FlushSource();

            if (!SupportsGuaranteedDelivery)
            {
                CloseSource();
            }
        }

        private void FlushSource()
        {
            try
            {
                if (LbmTemporarySource != null && !LbmTemporarySource.isClosed())
                {
                    LbmTemporarySource.flush();
                }
            }
            catch (Exception ex)
            {
                string txt = "Error flushing temporary source";

                Log.Error(ex, txt);
            }

            Thread.Sleep(10000);
        }

        [Obsolete("CloseSource is obsolete and Will be moved out to snapshot resolver")]
        protected void CloseSource()
        {
            try
            {
                Log.Info("Closing temporary source [" + TemporaryTopicName + "]");

                if (LbmTemporarySource != null && !LbmTemporarySource.isClosed())
                {
                    LbmTemporarySource.close();
                }
            }
            catch (Exception ex)
            {
                string txt = "Error closing temporary source";

                Log.ErrorException(txt, ex);
            }
        }

        protected override void SendMessage(MessageBase message, bool useGzip = false)
        {
            SendMessage(message, LbmTemporarySource, TemporaryTopicName, useGzip);
        }

        protected void SendPackage()
        {
            DeliveryContext.DataReady = true;
            CommandManager.AddCommand(SnapshotDeliveryCommand.FromSnapshotCommand(this));
        }

        protected void SendEndOfSnapshot()
        {
            if (this.SupportsGuaranteedDelivery)
            {
                SendMessage(new EndOfSnapshotMessage
                {
                    SentSequences = DeliveryContext.SnapshotData.SequenceNumbers.ToList(),
                    SnapshotId = DeliveryContext.SnapshotId
                });
            }
            else
            {
                SendMessage(new EndOfSnapshotMessage());
            }
        }

        protected virtual void SendSnapshotMessage<T, U>(
            T message,
            Func<T, IEnumerable<U>> getEntities,
            Func<T, IEnumerable<U>, T> getPartialMessage,
            int numEntities,
            int packSize) where T : SnapshotMessageBase
        {
            IEnumerable<U> entities = getEntities(message);

            int numPacks = (numEntities / packSize) + 1;

            for (int i = 0; i < numPacks; i++)
            {
                int skip = i * packSize;

                List<U> pack = entities.Skip(skip).Take(packSize).ToList();

                if (pack.Count > 0)
                {
                    T response = getPartialMessage(message, pack);

                    DeliveryContext.SnapshotData.AddMessage(response);
                }
            }

            SendPackage();
        }
    }
}