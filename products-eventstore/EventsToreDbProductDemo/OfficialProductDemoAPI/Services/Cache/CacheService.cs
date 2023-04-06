using EventStore.Client;
using Google.Api;
using MandaraDemo.GrpcDefinitions;
using MandaraDemoDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficialProductDemoAPI.Extensions;
using OfficialProductDemoAPI.Services.Contracts;
using Optional;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OfficialProductDemoAPI.Services.Cache

{

    public enum KnownEvents
    {
        UNKNOWN,
        Create,
        Update,
        Delete
    }

    public abstract class CacheService<T> : IDataService<T> where T:IReference
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


        abstract protected T? FromEventData(EventRecord record);


        public async Task EventHandler(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                try
                {
                    var item = FromEventData(evnt.Event);
                    if (item != null)
                    {
                        switch (evnt.Event.EventType.toKnownEvent())
                        {
                            case KnownEvents.Create:
                                if (!_cache.TryAdd(item.Id, item))
                                {
                                    Console.WriteLine("Failed to add {0} with id {1}", evnt.OriginalStreamId, item.Id);
                                }
                                break;

                            case KnownEvents.Update:
                                if (!_cache.TryUpdate(item.Id, item, _cache[item.Id]))
                                {
                                    Console.WriteLine("Failed to update {0} with id {1}", evnt.OriginalStreamId, item.Id);
                                }
                                break;
                            case KnownEvents.Delete:

                                Console.WriteLine("Delete not yet implemented");

                                break;
                            default:
                                Console.WriteLine("Unknown event {0}", evnt.Event.EventType);
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}", ex.Message);
                }
            }, cancellationToken).ContinueWith(t => LogEvent(subscription, evnt, cancellationToken));
        }


        async Task LogEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync(string.Format("{0} [{1}],{2}", DateTime.Now, subscription.SubscriptionId, new string(Encoding.UTF8.GetString(evnt.Event.Data.ToArray()))));
        }

        public async Task InitialLoad()
        {
            try
            {

                var productsSubs = _client.ReadStreamAsync(Direction.Forwards, this.StreamName, StreamPosition.Start);

                StreamPosition opPosition = 0;

                if (await productsSubs.ReadState == ReadState.StreamNotFound)
                {
                    throw new NoStreamException(this.StreamName);
                }
                await foreach (var e in productsSubs)
                {
                    opPosition = e.OriginalEventNumber;
                    var op = this.FromEventData(e.Event);// JsonSerializer.Deserialize<OfficialProduct>(Encoding.UTF8.GetString(e.Event.Data.ToArray()));
                    if (op!=null)
                    {
                        _cache.AddOrUpdate(op.Id, op, (key, oldValue) => op);
                    }
                }
                _ = await _client.SubscribeToStreamAsync(StreamName, FromStream.After(opPosition), EventHandler);
                Console.WriteLine("Initial load {0}: {1} ",StreamName,_cache.Count);
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
