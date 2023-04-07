using EventStore.Client;
using MandaraDemoDTO.Contracts;

namespace ProductsDemo7.Extensions
{
    public static class Helpers
    {
        public static EventData toEventData(this IEventData eData)
        {
            return new EventData(Uuid.FromGuid(Guid.NewGuid()),eData.Event.ToString(),eData.Data,eData.MetaData);
        }
    }
}
