using System;
using Mandara.Date;
using Newtonsoft.Json;

namespace Mandara.Business.Bus.Messages.Reporting
{
    public class QueuedReport
    {
        public string Entity { get; }
        public string Name { get; }
        public DateRange Dates { get; }
        public DateTime ScheduledAt { get; }
        public DateTime StartedAt { get; }

        [JsonIgnore]
        public static readonly DateTime DefaultStartTime = DateTime.MinValue;

        public QueuedReport(string entity, string name, DateRange dates, DateTime scheduledAt, DateTime startedAt)
        {
            Entity = entity;
            Name = name;
            Dates = dates;
            ScheduledAt = scheduledAt;
            StartedAt = startedAt;
        }

        public bool IsStarted() => DefaultStartTime != StartedAt;
    }
}
