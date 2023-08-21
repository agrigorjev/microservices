using Mandara.Business.Bus.Messages.Base;
using Mandara.Business.Services;

namespace Mandara.Business.Audit
{
    public class AuditContext
    {
        public string UserName { get; set; }
        public string UserIp { get; set; }
        public string Source { get; set; }
        public string ContextName { get; set; }

        private AuditContext(MessageBase message, string source, string context)
            : this(source, context, message.UserIp, message.UserName)
        {
        }

        public AuditContext()
        { }

        public AuditContext(string source, string context, string userIp, string userName)
        {
            Source = source;
            ContextName = context;
            UserIp = userIp;
            UserName = userName;
        }

        public static AuditContext CreateFromMessage(MessageBase message, string source, string context)
        {
            return new AuditContext(message, source, context);
        }
    }
}
