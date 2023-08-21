using System;

namespace Mandara.Entities
{
    public class NullTester
    {
        public static void ThrowIfNullArgument<T>(T toTest, string paramName, string errorMsgPrefix) where T : class
        {
            if (null == toTest)
            {
                throw new ArgumentNullException(paramName, $"{errorMsgPrefix} - {typeof(T).Name} is required.");
            }
        }
    }
}
