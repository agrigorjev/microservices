using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Mandara.Business.Bus;

namespace Mandara.Business.Configuration
{
    public class EntityReportingTopicsSection : ConfigurationSection
    {
        private const string SectionName = "entities";
        private const string ElemName = "";

        [ConfigurationProperty(ElemName, IsRequired = true, IsDefaultCollection = true)]
        //public ConfigurationElements<EntityTopic> Entities =>
        //    (ConfigurationElements<EntityTopic>)base[ElemName];
        public EntityTopics Entities => (EntityTopics)this[ElemName];

        private List<EntityTopic> _entities = new List<EntityTopic>();

        public IEnumerable<EntityTopic> GetEntities()
        {
            IEnumerator entityWalker = Entities.GetEnumerator();

            while (entityWalker.MoveNext())
            {
                yield return (EntityTopic)entityWalker.Current;
            }
        }

        public Dictionary<string, TopicDefinition> GetReceiveTopics(string action)
        {
            return GetEntityReceiveTopic(action);
        }

        private Dictionary<string, TopicDefinition> GetEntityReceiveTopic(string action)
        {
            return GetEntityTopic(
                action,
                (entity) => ConstructTopicDef(entity.Environment, entity.GetReceiveTopic(action)));
        }

        private TopicDefinition ConstructTopicDef(string env, string suffix)
        {
            return new TopicDefinition()
            {
                DefaultTopicNameSuffix = suffix,
                Environment = env ?? TopicDefinition.DefaultEnvironment,
            };
        }

        private Dictionary<string, TopicDefinition> GetEntityTopic(
            string action,
            Func<EntityTopic, TopicDefinition> getTopic)
        {
            if (!_entities.Any())
            {
                _entities = GetEntities().ToList();
            }

            return _entities.ToDictionary(entity => entity.Name, getTopic);
        }

        public Dictionary<string, TopicDefinition> GetQueueReportSendTopics()
        {
            return GetEntitySendTopic(Topic.QueueReport);
        }

        private Dictionary<string, TopicDefinition> GetEntitySendTopic(string action)
        {
            return GetEntityTopic(
                action,
                (entity) => ConstructTopicDef(entity.Environment, entity.GetSendTopic(action)));
        }

        public Dictionary<string, TopicDefinition> GetQueueReportReceiveTopics()
        {
            return GetEntityReceiveTopic(Topic.QueueReport);
        }

        public Dictionary<string, TopicDefinition> GetSendTopics(string action)
        {
            return GetEntitySendTopic(action);
        }

        public Dictionary<string, TopicDefinition> GetCancelReportSendTopics()
        {
            return GetEntitySendTopic(Topic.CancelReport);
        }

        public Dictionary<string, TopicDefinition> GetCancelReportReceiveTopics()
        {
            return GetEntityReceiveTopic(Topic.CancelReport);
        }

        public Dictionary<string, TopicDefinition> GetReportQueueSendTopics()
        {
            return GetEntitySendTopic(Topic.ReportQueue);
        }

        public Dictionary<string, TopicDefinition> GetReportQueueReceiveTopics()
        {
            return GetEntityReceiveTopic(Topic.ReportQueue);
        }

        public static EntityReportingTopicsSection Get()
        {
            return (EntityReportingTopicsSection)ConfigurationManager.GetSection(SectionName);
        }
    }

    [ConfigurationCollection(typeof(EntityTopic), AddItemName = EntityTopic.ElemName)]
    public class EntityTopics : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EntityTopic();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntityTopic)element).Name;
        }
    }
}
