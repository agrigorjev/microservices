using System.Collections.Concurrent;
using Mandara.Business.Contracts;

namespace Mandara.Business.Services
{
    public class SequenceNumbersInMemoryCache : ISequenceNumbersCache
    {
        public ConcurrentDictionary<string, int> Values { get; private set; }

        public SequenceNumbersInMemoryCache()
        {
            Values = new ConcurrentDictionary<string, int>();
        }
    }
}