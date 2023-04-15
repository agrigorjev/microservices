using MandaraDemo.GrpcDefinitions;
using OfficialProductDemoAPI.Services.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OfficialProductDemoAPI.Services
{
    public class ServiceNotification : INotifyService<ServiceEventMessage>
    {

        private readonly Subject<ServiceEventMessage> _serviceEvents;

        public ServiceNotification()
        {
            _serviceEvents = new Subject<ServiceEventMessage>();
        }

        public Subject<ServiceEventMessage> eventSubject => _serviceEvents;

        public void QueueEvent(ServiceEventMessage eventData)
        {
            _serviceEvents.OnNext(eventData);
        }
    }
}
