using System.Collections.Concurrent;
using Mandara.Business.Model;

namespace Mandara.Business.Contracts
{
    public interface ILiveMessagesCache
    {
        ConcurrentDictionary<string, MessageCache> MessagesCache { get; }
    }
}