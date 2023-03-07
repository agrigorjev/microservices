using System;

namespace Mandara.Business.PublishSubscribe.Events.Alerts
{
    public class AlertsSnapshotEvent<T> where T : class
    {
        public string Username { get; set; }

        public Action<T> Callback { get; set; }

        public AlertsSnapshotEvent(string userName, Action<T> callback)
        {
            Username = userName;
            Callback = callback;
        }
    }
}