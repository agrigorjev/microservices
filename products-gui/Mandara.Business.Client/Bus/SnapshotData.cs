using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Business.Bus.Messages.Base;
using com.latencybusters.lbm;
using NLog;

namespace Mandara.Business.Bus
{
    public class SnapshotDeliveryContext
    {
        public SnapshotDeliveryContext(string topicName, Guid snapshotId, SnapshotData snapshotData)
        {
            TopicName = topicName;
            SnapshotId = snapshotId;
            SnapshotData = snapshotData;
        }

        public SnapshotData SnapshotData { get; private set; }
        public Guid SnapshotId { get; private set; }
        public string TopicName { get; private set; }
        public string TempTopicName { get { return string.Format("{0}/{1}", TopicName, SnapshotId); } }
        public bool DataReady { get; set; }
    }

    public class SnapshotData
    {
        private readonly ConcurrentDictionary<int, SnapshotMessageBase> _messages = new ConcurrentDictionary<int, SnapshotMessageBase>();
        private Logger Log = LogManager.GetCurrentClassLogger();

        public SnapshotData() { }

        public SnapshotData(SnapshotMessageBase[] messages)
        {
            int sequenceNumber = 0;
            _messages = new ConcurrentDictionary<int, SnapshotMessageBase>(messages.ToDictionary(m => sequenceNumber++, m => m));
            _sequenceNumber = sequenceNumber;
        }

        private int _sequenceNumber = 0;
        public SnapshotMessageBase this[int sequenceNumber]
        {
            get
            {
                SnapshotMessageBase message;
                _messages.TryGetValue(sequenceNumber, out message);

                return message;
            }
        }

        public void AddMessage(SnapshotMessageBase message)
        {
            message.SequenceNumber = _sequenceNumber++;
            if (!_messages.TryAdd(message.SequenceNumber, message))
            {
                throw new Exception("Couldn't add message to SnapshotData due to data race.");
            }
        }

        public IEnumerable<int> SequenceNumbers { get { return _messages.Keys; } }

        public void RemoveMessages(List<int> sequenceNumbers)
        {
            Log.Debug("Removing sequence numbers: " + string.Join(",", sequenceNumbers));
            foreach (var sequenceNumber in sequenceNumbers)
            {
                SnapshotMessageBase ignore;
                _messages.TryRemove(sequenceNumber, out ignore);
            }
        }
    }
}
