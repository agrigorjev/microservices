using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Entities.ErrorDetails
{
    public class FailureCallbackDetails : ErrorDetails
    {
        public FailureCallbackDetails(FailureCallbackInfo info)
        {
            MessageTypeName = info.MessageTypeName;
            TopicName = info.TopicName;
        }

        [Description("Bus message type name")]
        [DisplayName("Message Type Name")]

        public string MessageTypeName { get; set; }

        [Description("Bus topic name which used in failed request")]
        [DisplayName("Topic Name")]
        public string TopicName { get; set; }

    }
}
