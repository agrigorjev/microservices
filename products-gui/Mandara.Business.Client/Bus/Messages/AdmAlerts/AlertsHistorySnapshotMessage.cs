using System;
using System.Collections.Generic;
using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities;

namespace Mandara.Business.Bus.Messages.AdmAlerts
{
    public class AlertsHistorySnapshotMessage : SnapshotMessageBase
    {
        public DateTime Date { get; set; } 

        public List<AlertHistory> AlertHistories { get; set; }
    }
}