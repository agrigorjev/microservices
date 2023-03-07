using com.latencybusters.lbm;
using Mandara.Business.Bus.Messages.Base;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.SnapshotReceivers
{
    public class SnapshotPackageReceiver<T> : SnapshotReceiver<T> where T : SnapshotMessageBase
    {
        // TODO: rework to pass callback down to SnapshotReceiver so that it would be transferred as argument until
        // SnapshotEnd
        private Action<List<T>> _callback;
        
        public SnapshotPackageReceiver(
            LBMContext lbmContext,
            LBMSource lbmSnapshotSource,
            string topicName,
            int responseTimeout)
                : base(lbmContext, lbmSnapshotSource, topicName, responseTimeout)
        {
        }

        public void GetSnapshot(
            T message, 
            Action<List<T>> callback,
            Action snapshotReceivedCallback,
            Action snapshotFailureCallback)
        {
            _callback = callback;
            GetSnapshot(message, OnSnapshotReceived, snapshotReceivedCallback, snapshotFailureCallback);
        }

        private readonly List<T> _receivedMessages = new List<T>();
        protected void OnSnapshotReceived(T message)
        {
            lock (_receivedMessages)
            {
                _receivedMessages.Add(message);
            }
        }

        protected override void OnSnapshotEnd()
        {
            lock (_receivedMessages)
            {
                if (_callback != null)
                {
                    _callback(_receivedMessages);
                }
            }
        }
    }
}
