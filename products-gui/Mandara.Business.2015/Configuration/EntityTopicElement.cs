using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Mandara.Business.Config;

namespace Mandara.Business.Configuration
{
    public class EntityTopic : ConfigurationElement
    {
        private List<Topic> _receiveTopics = new List<Topic>();
        private List<Topic> _sendTopics = new List<Topic>();
        public const string ElemName = "entity";
        public const string EntityName = "name";
        public const string EntityEnvironment = "environment";
        public const string IncomingTopicsProperty = "receive";
        public const string OutgoingTopicsProperty = "send";

        [ConfigurationProperty(EntityName, IsRequired = true)]
        public string Name
        {
            get => (string)this[EntityName];
            set => this[EntityName] = value;
        }

        [ConfigurationProperty(EntityEnvironment, IsRequired = false)]
        public string Environment
        {
            get => (string)this[EntityEnvironment];
            set => this[EntityEnvironment] = value;
        }

        [ConfigurationProperty(IncomingTopicsProperty)]
        public RequestTopics Receives
        {
            get => (RequestTopics)this[IncomingTopicsProperty];
            set => this[IncomingTopicsProperty] = value;
        }

        [ConfigurationProperty(OutgoingTopicsProperty)]
        public ResponseTopics Sends
        {
            get => (ResponseTopics)this[OutgoingTopicsProperty];
            set => this[OutgoingTopicsProperty] = value;
        }

        public override string ToString()
        {
            return Name;
        }

        private List<Topic> GetReceiveTopics()
        {
            if (_receiveTopics.Any())
            {
                return _receiveTopics;
            }

            _receiveTopics = GetTopics(Receives).ToList();
            return _receiveTopics;
        }

        private IEnumerable<Topic> GetTopics(ConfigurationElementCollection topics)
        {
            IEnumerator topicWalker = topics.GetEnumerator();

            while (topicWalker.MoveNext())
            {
                yield return (Topic)topicWalker.Current;
            }
        }

        private List<Topic> GetSendTopics()
        {
            if (_sendTopics.Any())
            {
                return _sendTopics;
            }

            _sendTopics = GetTopics(Sends).ToList();
            return _sendTopics;
        }

        public string GetReceiveTopic(string action)
        {
            return GetReceiveTopics().First(topic => action == topic.TopicAction).Suffix;
        }

        public string GetSendTopic(string action)
        {
            return GetSendTopics().First(topic => action == topic.TopicAction).Suffix;
        }
    }

    [ConfigurationCollection(typeof(Topic), AddItemName = Topic.ElemName)]
    public class RequestTopics : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Topic();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return $"{((Topic)element).TopicAction}{((Topic)element).Suffix}";
        }
    }

    [ConfigurationCollection(typeof(Topic), AddItemName = Topic.ElemName)]
    public class ResponseTopics : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Topic();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return $"{((Topic)element).TopicAction}{((Topic)element).Suffix}";
        }
    }

    public class Topic : ConfigurationElement
    {
        public const string QueueReport = "queueReport";
        public const string CancelReport = "cancel";
        public const string QueueReportResult = "queueReportResult";
        public const string CancelReportResult = "cancelResult";
        public const string ReportQueue = "reportQueue";

        public const string ElemName = "topic";
        public const string ActionProperty = "action";
        public const string SuffixProperty = "suffix";

        [ConfigurationProperty(ActionProperty, IsRequired = true)]
        public string TopicAction
        {
            get => (string)this[ActionProperty];
            set => this[ActionProperty] = value;
        }

        [ConfigurationProperty(SuffixProperty, IsRequired = true)]
        public string Suffix
        {
            get => (string)this[SuffixProperty];
            set => this[SuffixProperty] = value;
        }
    }
}
