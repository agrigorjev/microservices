using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages
{
    public class LiveFeedReplaySnapshotMessage : SnapshotMessageBase
    {
        public string TopicName { get; set; }
        public List<int> Sequences { get; set; }

        public List<MessageBase> DataMessages { get; set; }
    }

    public class LiveFeedReplaySnapshotMessageDto : SnapshotMessageBase
    {
        public string TopicName { get; set; }
        public List<int> Sequences { get; set; }

        public List<MessageBase> DataMessages { get; set; }
    }
}