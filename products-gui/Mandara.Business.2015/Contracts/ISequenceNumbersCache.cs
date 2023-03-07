using System.Collections.Concurrent;

namespace Mandara.Business.Contracts
{
    public interface ISequenceNumbersCache
    {
        ConcurrentDictionary<string, int> Values { get; }
    }
}