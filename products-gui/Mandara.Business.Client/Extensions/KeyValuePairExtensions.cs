using System;
using System.Collections.Generic;

namespace Mandara.Business.Client.Extensions
{
    [Obsolete("Move to Mandara.Extensions")]
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<T, U>(this KeyValuePair<T, U> pairToDeconstruct, out T key, out U value)
        {
            key = pairToDeconstruct.Key;
            value = pairToDeconstruct.Value;
        }
    }
}
