using System.Collections.Concurrent;

namespace Mandara.Entities.Extensions
{
    public static class ConcurrentBagExtensions
    {
        public static void AddRange<T>(this ConcurrentBag<T> bag, ConcurrentBag<T> otherBag)
        {
            foreach (T val in otherBag.ToArray())
            {
                bag.Add(val);
            }
        }
    }
}