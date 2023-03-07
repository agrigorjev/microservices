using Mandara.Business.Audit;
using Mandara.Business.Bus.Messages.Audit;
using Mandara.Date.Time;
using Mandara.Entities;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Mandara.Business
{
    using Mandara.Business.Bus.Messages.Base;
    using Mandara.Proto.Encoder;
    using System;

    public class AuditManager
    {
        public static void WriteAuditMessage(string userName, string userIp, string source, string contextName, string messageType, string objectDescription = null)
        {
            if (userIp == null)
            {
                IPAddress currentIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                if (currentIp != null)
                    userIp = currentIp.ToString();
            }

            var auditMessage = new AuditMessage
            {
                UserName = userName,
                UserIp = userIp,
                Source = source,
                ContextName = contextName,
                MessageType = messageType,
                MessageTime = SystemTime.Now(),
                ObjectDescription = objectDescription,
            };

            using (var cxt = new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(AuditManager)))
            {
                cxt.AuditMessages.Add(auditMessage);
                cxt.SaveChanges();
            }
        }

        public static AuditContext CreateAuditContext(MessageBase message, string source, string contextName)
        {
            return new AuditContext
            {
                UserName = message.UserName,
                UserIp = message.UserIp,
                Source = source,
                ContextName = contextName,
            };
        }

        public static void CreateAuditMessage(MandaraEntities cxt, WriteAuditRequestMessage message)
        {
            if (message == null)
                return;

            AuditMessage auditMessage = new AuditMessage
            {
                MessageTime = SystemTime.Now(),
                MessageType = message.MessageType,
                ContextName = message.ContextName,
                Source = message.Source,
                ObjectDescription = message.ObjectDescription,
                UserName = message.UserName,
                UserIp = message.UserIp,
            };

            if (!String.IsNullOrEmpty(message.PropertyName))
            {
                AuditMessageDetails details = new AuditMessageDetails()
                {
                    Property = message.PropertyName,
                    OldValue = message.PreviousValue,
                    NewValue = message.CurrentValue
                };
                auditMessage.Details = details;
                ProtoEncoder.EncodeDetailsAsGzipProto(auditMessage);
            }

            cxt.AuditMessages.Add(auditMessage);
        }
    }
}
