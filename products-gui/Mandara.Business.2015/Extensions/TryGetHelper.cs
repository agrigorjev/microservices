using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandara.Extensions.Option;

namespace Mandara.Business.Extensions
{
    public static class TryGetHelper
    {
        public static TryGetRef<T> FromDict<T, K>(Dictionary<K, T> values, K key) where T : class
        {
            if (values.ContainsKey(key))
            {
                return new TryGetRef<T>(values[key]);
            }
            else
            {
                return new TryGetRef<T>();
            }

        }

        public static TryGetResult<T> TryGetRefFromObject<T, U>(T inputObject, Func<T, U> keySelector, U key)
            where T : class
        {
            U inputKey = keySelector(inputObject);
            return key.Equals(inputKey) ? new TryGetRef<T>(inputObject) : new TryGetRef<T>();
        }
    }
}
