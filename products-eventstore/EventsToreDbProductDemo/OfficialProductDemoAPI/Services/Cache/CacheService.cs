using EventStore.Client;
using Google.Api;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using MandaraDemoDTO.Contracts;
using MandaraDemoDTO.Extensions;
using NLog;
using NLog.Fluent;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using static EventStore.Client.StreamMessage;


namespace OfficialProductDemoAPI.Services.Cache

{
    public abstract class CacheService<T> : IDataService<T> where T : IReference, IState
    {
        protected ConcurrentDictionary<Guid, T> _cache = new ConcurrentDictionary<Guid, T>();

        protected EventStoreClient _client;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected bool _warmupDone = false;
        public abstract INotifyService<ServiceEventMessage>? NotificationService { get; set; }

        public abstract string StreamName { get; }

        public List<T> GetList()
        {
            return _cache.Values.ToList();
        }

        public Option<T> GetSingle(string id)
        {
            if (Guid.TryParse(id, out Guid key))
            {
                return _cache.ContainsKey(key) ? Option.Some(_cache[key]) : Option.None<T>();
            }
            return Option.None<T>();
        }


        public void EventHandler(EventRecord evnt, string stream)
        {
            try
            {
                var item = ObjectEvent<T>.fromEventData(evnt);
                if (item != null)
                {
                    switch (evnt.EventType.toKnownEvent())
                    {
                        case KnownEvents.Create:
                            if (!_cache.TryAdd(item.Id, item.Subject))
                            {
                                _logger.Warn("Create failed {0} for id [{1}], event {2}", stream, item.Id, evnt.EventId);
                            }

                            break;

                        case KnownEvents.Update:
                            if (_cache.TryGetValue(item.Id, out T toUpdate))
                            {
                                if (!_cache.TryUpdate(item.Id, item.withUpdate(toUpdate), _cache[item.Id]))
                                {
                                    _logger.Warn("Update failed {0} for id [{1}], event {2}", stream, item.Id, evnt.EventId);
                                }
                                else
                                {
                                    _logger.Warn("{0} for update not found {1} of event {2}", stream, item.Id, evnt.EventId);
                                }

                            }
                            break;
                        case KnownEvents.Delete:

                            if (_cache.ContainsKey(item.Id))
                            {
                                _cache[item.Id].Status = State.REMOVED;
                            }
                            else
                            {
                                _logger.Warn("{0} for delete not found {1} of event {2}", stream, item.Id, evnt.EventId);
                            }


                            break;
                        default:
                            _logger.Warn("Unknown event \"{0}\" of event:{1}", evnt.EventType, evnt.EventId);

                            break;
                    }
                    if (_warmupDone && NotificationService != null)
                    {
                        var eventMessage = new ServiceEventMessage()
                        {
                            EventType = typeof(T).Name
                        };
                        eventMessage.EventPayload.Add(item.Id.ToString());
                        NotificationService.QueueEvent(eventMessage);
                    }
                }
                _logger.Trace(("{0} [{1}],{2}", DateTime.Now, stream, new string(Encoding.UTF8.GetString(evnt.Data.ToArray()))));

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Event handle error {0} {1}", evnt.EventStreamId, evnt.EventId);
            }
        }

        public async Task InitialLoad()
        {
            try
            {

                var productsSubs =
                    _client.ReadStreamAsync(Direction.Forwards, this.StreamName, StreamPosition.Start);

                StreamPosition opPosition = 0;

                if (await productsSubs.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException(this.StreamName);
                }
                else
                {

                    productsSubs.ToObservable<ResolvedEvent>()
                    .Aggregate(opPosition, (_, e) =>
                    {
                        EventHandler(e.Event, e.OriginalEvent.EventStreamId);
                        return e.OriginalEventNumber;
                    })
                    .Subscribe(
                        e =>
                        {
                            _client
                            .SubscribeToStreamAsync(StreamName,
                                FromStream.After(e),
                                (stream, evt, token) => Task.Run(() => EventHandler(evt.Event, evt.OriginalEvent.EventStreamId)));

                            _logger.Info("Load {0}: {1} ", StreamName, _cache.Count);
                            _warmupDone = true;
                            if (NotificationService != null)
                            {
                                var eventMessage = new ServiceEventMessage()
                                {
                                    EventType = typeof(T).Name
                                };
                                eventMessage.EventPayload.Add("Warmup!");
                                NotificationService.QueueEvent(eventMessage);
                            }

                        }
                        );
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Initial load error [{0}]",StreamName);
            }
            return;

        }

        public void Dispose()
        {

        }
    }
}
