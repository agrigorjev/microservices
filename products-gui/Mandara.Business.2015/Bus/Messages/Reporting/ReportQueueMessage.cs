using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;

namespace Mandara.Business.Bus.Messages.Reporting
{
    public class ReportQueueMessage : MessageBase
    {
        public string Entity { get; }
        public List<QueuedReport> Reports { get; }

        public ReportQueueMessage(string entity, List<QueuedReport> reports)
        {
            Entity = entity;
            Reports = new List<QueuedReport>(reports);
        }
    }
}
