using Mandara.Business.Audit;
using Mandara.Business.Extensions;
using Mandara.Date.Time;
using Mandara.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Mandara.Proto.Encoder;

namespace Mandara.Business.Services
{
    public class AuditService : IAuditService
    {
        private readonly ConcurrentDictionary<string, Func<dynamic, object>> _typeKeyGetters = new ConcurrentDictionary<string, Func<dynamic, object>>();

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Func<dynamic, object>>> _typePropsGetters =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, Func<dynamic, object>>>();

        public List<AuditMessage> CreateAuditMessages<T>(AuditContext auditContext, string messageType, List<T> originalEntities, List<T> modifiedEntities) where T : class
        {
            if (originalEntities == null && modifiedEntities == null)
                return null;

            Type type = typeof(T);
            List<AuditMessage> auditMessages = new List<AuditMessage>();

            Dictionary<int, T> originalMap = new Dictionary<int, T>();
            Dictionary<int, T> modifiedMap = new Dictionary<int, T>();

            FillMap(originalEntities, originalMap, type);
            FillMap(modifiedEntities, modifiedMap, type);

            List<int> keys = originalMap.Keys.Union(modifiedMap.Keys).Distinct().ToList();

            foreach (int key in keys)
            {
                T orig, mod;
                originalMap.TryGetValue(key, out orig);
                modifiedMap.TryGetValue(key, out mod);

                if (orig == null && mod == null)
                    continue;

                auditMessages.Add(CreateAuditMessage(auditContext, messageType, orig, mod));
            }

            return auditMessages;
        }

        public List<AuditMessage> UpdateAuditMessages<T>(List<AuditMessage> auditMessages, List<T> modifiedEntities) where T : class
        {
            Type type = typeof(T);

            Dictionary<int, T> modifiedMap = new Dictionary<int, T>();
            FillMap(modifiedEntities, modifiedMap, type);

            Dictionary<int, AuditMessage> auditMessagesMap = auditMessages.ToDictionary(x => x.ObjectId ?? 0);

            foreach (int key in modifiedMap.Keys)
            {
                AuditMessage am;
                auditMessagesMap.TryGetValue(key, out am);
                T mod;
                modifiedMap.TryGetValue(key, out mod);

                if (am == null)
                    continue;

                UpdateAuditMessage(am, mod);
            }

            return auditMessages;
        }

        public AuditMessage CreateAuditMessage<T>(AuditContext auditContext, string messageType, T originalEntity, T modifiedEntity) where T : class
        {
            if (originalEntity == null && modifiedEntity == null)
                return null;

            Type type = typeof(T);

            var auditMessage = new AuditMessage
            {
                UserIp = auditContext.UserIp,
                UserName = auditContext.UserName,
                Source = auditContext.Source,
                ContextName = auditContext.ContextName,
                ObjectType = type.Name,
                MessageType = messageType,
                MessageTime = SystemTime.Now(),
                ObjectDescription = (modifiedEntity ?? originalEntity).ToString(),
                ObjectId = GetKeyValue(type, originalEntity ?? modifiedEntity),
                Details = GetDetails(type, originalEntity, modifiedEntity)
            };

           ProtoEncoder.EncodeDetailsAsGzipProto(auditMessage);

            return auditMessage;
        }

        public AuditMessage UpdateAuditMessage<T>(AuditMessage auditMessage, T modifiedEntity) where T : class
        {
            auditMessage.Details = GetDetails(typeof(T), auditMessage, modifiedEntity);

            RemoveEqualDetails(auditMessage.Details);

            ProtoEncoder.EncodeDetailsAsGzipProto(auditMessage);

            return auditMessage;
        }

        public void RemoveEqualDetails(AuditMessageDetails details)
        {
            foreach (var child in details.Children.ToList())
            {
                if (child.Children != null)
                    RemoveEqualDetails(child);

                if ((child.Children == null || child.Children.Count == 0) && child.OldValue == child.NewValue)
                {
                    details.Children.Remove(child);
                }
            }
        }

        private void FillMap<T>(List<T> entities, Dictionary<int, T> map, Type type) where T : class
        {
            if (entities != null && entities.Count > 0)
            {
                foreach (var originalEntity in entities)
                {
                    map.Add(GetKeyValue(type, originalEntity) ?? 0, originalEntity);
                }
            }
        }

        private AuditMessageDetails GetDetails<T>(Type type, AuditMessage auditMessage, T modifiedEntity)
        {
            AuditMessageDetails baseAuditDetails = auditMessage.Details;

            ConcurrentDictionary<string, Func<dynamic, object>> propGetter = GetPropertiesDictionary(type);

            foreach (var pair in propGetter.ToArray())
            {
                string propertyName = pair.Key;
                AuditMessageDetails parentDetail = baseAuditDetails;

                if (propertyName.Contains("."))
                {
                    string[] parts = propertyName.Split('.');
                    propertyName = parts[1];

                    parentDetail = baseAuditDetails.Children.FirstOrDefault(x => x.Property == parts[0]);
                    if (parentDetail == null)
                    {
                        continue;
                    }
                }

                var childDetail = parentDetail.Children.FirstOrDefault(x => x.Property == propertyName);

                if (childDetail == null)
                    continue;

                object modifiedValue = pair.Value(modifiedEntity);
                string modifiedString = modifiedValue == null ? string.Empty : modifiedValue.ToString();
                childDetail.NewValue = modifiedString;
            }

            return baseAuditDetails;
        }

        private AuditMessageDetails GetDetails<T>(Type type, T originalEntity, T modifiedEntity)
        {
            AuditMessageDetails baseAuditDetails = new AuditMessageDetails();

            ConcurrentDictionary<string, Func<dynamic, object>> propGetter = GetPropertiesDictionary(type);

            foreach (var pair in propGetter.ToArray())
            {
                string propertyName = pair.Key;
                AuditMessageDetails parentDetail = baseAuditDetails;

                if (propertyName.Contains("."))
                {
                    string[] parts = propertyName.Split('.');
                    propertyName = parts[1];

                    parentDetail = baseAuditDetails.Children.FirstOrDefault(x => x.Property == parts[0]);
                    if (parentDetail == null)
                    {
                        parentDetail = new AuditMessageDetails
                        {
                            Property = parts[0]
                        };

                        baseAuditDetails.Children.Add(parentDetail);
                    }
                }

                object originalValue = originalEntity == null ? null : pair.Value(originalEntity);
                object modifiedValue = modifiedEntity == null ? null : pair.Value(modifiedEntity);

                string originalString = originalValue == null ? string.Empty : originalValue.ToString();
                string modifiedString = modifiedValue == null ? string.Empty : modifiedValue.ToString();

                var childDetail = new AuditMessageDetails
                {
                    Property = propertyName,
                    OldValue = originalString,
                    NewValue = modifiedString
                };

                parentDetail.Children.Add(childDetail);
            }

            return baseAuditDetails;
        }

        private ConcurrentDictionary<string, Func<dynamic, object>> GetPropertiesDictionary(Type type, bool auditMasters = true)
        {
            ConcurrentDictionary<string, Func<dynamic, object>> propGetter;

            if (!_typePropsGetters.TryGetValue(type.Name, out propGetter))
            {
                propGetter = new ConcurrentDictionary<string, Func<dynamic, object>>();
                _typePropsGetters.TryAdd(type.Name, propGetter);

                foreach (PropertyInfo propInfo in type.GetProperties())
                {
                    if (propInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Any())
                    {
                        string propName = propInfo.Name;

                        Func<dynamic, object> func;
                        if (!propGetter.TryGetValue(propName, out func))
                        {
                            func = propInfo.GetValueGetter();
                            propGetter.TryAdd(propName, func);
                        }
                    }

                    if (!auditMasters)
                        continue;

                    if (propInfo.GetCustomAttributes(typeof(ForeignKeyAttribute), false).Any())
                    {
                        var innerPropGetter = GetPropertiesDictionary(propInfo.PropertyType, false);

                        PropertyInfo closurePropInfo = propInfo;
                        foreach (var pair in innerPropGetter)
                        {
                            Func<dynamic, object> innerPropFunc = pair.Value;
                            propGetter.TryAdd(propInfo.Name + "." + pair.Key,
                                x =>
                                {
                                    dynamic value = closurePropInfo.GetValue(x, null);
                                    if (value == null)
                                        return null;
                                    return innerPropFunc(value);
                                });
                        }
                    }
                }
            }

            return propGetter;
        }

        private int? GetKeyValue<T>(Type type, T entity)
        {
            Func<dynamic, object> func;
            if (!_typeKeyGetters.TryGetValue(type.Name, out func))
            {
                func = GetKeyPropertyInfo(type).GetValueGetter();
                _typeKeyGetters.TryAdd(type.Name, func);
            }

            if (func == null)
                return null;

            return (int?)func(entity);
        }

        private PropertyInfo GetKeyPropertyInfo(Type type)
        {
            return (from p in type.GetProperties()
                    where p.GetCustomAttributes(typeof(KeyAttribute), false).Any()
                    select p).ToList().First();
        }
    }
}