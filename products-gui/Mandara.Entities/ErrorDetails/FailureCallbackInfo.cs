using System;

namespace Mandara.Entities.ErrorDetails
{
    public class FailureCallbackInfo
    {
        private const string NoMsgType = "Unspecified Msg Type";
        public string MessageTypeName { get; set; } = NoMsgType;
        private const string NoTopic = "Unspecified Topic";
        public string TopicName { get; set; } = NoTopic;
        public string AdditionalInfo { get; }

        public FailureCallbackInfo()
        {
            AdditionalInfo = "";
        }

        public FailureCallbackInfo(string additionalInfo)
        {
            AdditionalInfo = additionalInfo;
        }

        public FailureCallbackInfo(string messageType, string topic, string additionalInfo)
        {
            MessageTypeName = messageType ?? NoMsgType;
            TopicName = topic ?? NoTopic;
            AdditionalInfo = additionalInfo ?? "";
        }

        public bool HasAdditionalInfo => !String.IsNullOrWhiteSpace(AdditionalInfo);

        public override string ToString()
        {
            return HasAdditionalInfo
                ? $"{nameof(MessageTypeName)}: {MessageTypeName}, on {nameof(TopicName)}: {TopicName} - {AdditionalInfo}"
                : $"{nameof(MessageTypeName)}: {MessageTypeName}, on {nameof(TopicName)}: {TopicName}";
        }
    }
}
