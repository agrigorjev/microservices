using Mandara.Business.Bus;
using Mandara.Business.Bus.Handlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace Mandara.Business.Configuration
{
    public class QueueReportEntitiesAndTopics<T> where T : IHandler
    {
        private EntityReportingTopicsSection _entityTopics = EntityReportingTopicsSection.Get();
        private Dictionary<string, string> _sendTopicsByEntity;
        private Dictionary<string, (string topic, Type handler)> _receiveTopicsByEntity;
        // This is not const or static because C# doesn't allow access to static or const via an instance and since this
        // is a generic class, that's painful.
        public string NoTopic = String.Empty;

        public QueueReportEntitiesAndTopics(Func<TopicDefinition, string> configTopicToBusTopic)
        {
            GetEntityBusTopics(configTopicToBusTopic);
        }

        private void GetEntityBusTopics(Func<TopicDefinition, string> configTopicToBusTopic)
        {
            _sendTopicsByEntity = _entityTopics.GetSendTopics(Topic.QueueReport).ToDictionary(
                entityTopic => entityTopic.Key,
                entityTopic => configTopicToBusTopic(entityTopic.Value));
            _receiveTopicsByEntity = _entityTopics.GetReceiveTopics(Topic.QueueReport).ToDictionary(
                entityTopic => entityTopic.Key,
                entityTopic => (configTopicToBusTopic(entityTopic.Value), typeof(T)));
        }

        public List<string> GetOutgoingTopics()
        {
            return _sendTopicsByEntity.Values.ToList();
        }

        public List<Tuple<string, Type>> GetIncomingTopics()
        {
            return _receiveTopicsByEntity.Values.Select(
                topicAndHandler => new Tuple<string, Type>(
                    topicAndHandler.topic,
                    topicAndHandler.handler)).ToList();
        }

        public string TryGetSendTopicForEntity(string entity)
        {
            return _sendTopicsByEntity.TryGetValue(entity, out string topic)
                ? topic
                : NoTopic;
        }

        public List<string> GetEntityNames()
        {
            return _entityTopics.GetEntities().Select(entity => entity.Name).ToList();
        }
    }
}
