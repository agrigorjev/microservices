using EventStore.Client;
using Optional;
using System.Reactive.Subjects;
using System.Text;

namespace OfficialProductDemoAPI.Services.Contracts
{
    public interface INotifyService<T>
    {
        void QueueEvent(T eventData);

        public Subject<T> eventSubject{get;}

    }
}
