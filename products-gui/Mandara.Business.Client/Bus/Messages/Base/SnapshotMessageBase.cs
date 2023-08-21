using System;
using System.Collections.Generic;

namespace Mandara.Business.Bus.Messages.Base
{
    [Serializable]
    public abstract class SnapshotMessageBase : MessageWithErrorBase
    {
        public Guid? SnapshotId { get; set; }

        protected SnapshotMessageBase()
        {
            SnapshotMessageType = SnapshotMessageType.Data;
        }

        protected SnapshotMessageBase(Guid snapshotId)
            : this()
        {
            SnapshotId = snapshotId;
        }

        public SnapshotMessageType SnapshotMessageType { get; set; }

        public List<int> SentSequences { get; set; }
        public List<int> ReceivedSequences { get; set; }

        public bool UseGuaranteedDelivery { get; set; }

        public override void OnErrorSet()
        {
        }
    }
}