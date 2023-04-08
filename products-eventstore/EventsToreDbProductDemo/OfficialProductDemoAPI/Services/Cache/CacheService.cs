using EventStore.Client;
using Google.Api;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using MandaraDemoDTO.Contracts;
using MandaraDemoDTO.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficialProductDemoAPI.Extensions;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OfficialProductDemoAPI.Services.Cache

{



    public abstract class CacheService<T> : IDataService<T> where T:IReference,IState
    {
        protected ConcurrentDictionary<Guid, T> _cache = new ConcurrentDictionary<Guid, T>();

        protected EventStoreClient _client;

        public abstract string StreamName { get ;  }

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


        public void EventHandler(EventRecord evnt,string stream)
        {
          try
                {
                    var item = ObjectEvent<T>.fromEventData(evnt);
                    //var item = FromEventData(evnt.Event);
                    if (item != null)
                    {
                        switch (evnt.EventType.toKnownEvent())
                        {
                            case KnownEvents.Create:
                                if (!_cache.TryAdd(item.Id, item.Subject))
                                {
                                    Console.WriteLine("Failed to add {0} with id {1}", stream, item.Id);
                                }
                                break;

                            case KnownEvents.Update:
                                if (!_cache.TryUpdate(item.Id, item.Subject, _cache[item.Id]))
                                {
                                    Console.WriteLine("Failed to update {0} with id {1}", stream, item.Id);
                                }
                                break;
                            case KnownEvents.Delete:

                                if (_cache.ContainsKey(item.Id))
                                {
                                    _cache[item.Id].Status = State.REMOVED;
                                }

                                break;
                            default:
                                Console.WriteLine("Unknown event {0}", evnt.EventType);
                                break;
                        }
                    }
                    Console.Out.WriteLineAsync(string.Format("{0} [{1}],{2}", DateTime.Now,stream, new string(Encoding.UTF8.GetString(evnt.Data.ToArray()))));
                 }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
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
                    .SubscribeOn(Scheduler.Immediate)
                    .Subscribe(
                        e =>
                        {
                            _client
                            .SubscribeToStreamAsync(StreamName,
                                FromStream.After(e),
                                (stream, evt, token) => Task.Run(() => EventHandler(evt.Event, evt.OriginalEvent.EventStreamId)));
                                Console.WriteLine("Load {0}: {1} ", StreamName, _cache.Count);
                          
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return;

        }

        public void Dispose()
        {

        }
    }
}
