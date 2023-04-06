using System.Runtime.Serialization;

namespace OfficialProductDemoAPI
{
    [Serializable]
    internal class NoStreamException : Exception
    {
        public NoStreamException()
        {
        }

        public NoStreamException(string message) : base(string.Format("Stream: <{0}> not found.",message))
        {
        }

        public NoStreamException(string message, Exception? innerException) : base(string.Format("Stream: <{0}> not found.", message), innerException)
        {
        }

        protected NoStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}