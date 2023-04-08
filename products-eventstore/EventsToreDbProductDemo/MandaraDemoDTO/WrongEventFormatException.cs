using System.Runtime.Serialization;

namespace MandaraDemoDTO
{
    [Serializable]
    internal class WrongEventFormatException : Exception
    {
        public WrongEventFormatException()
        {
        }

        public WrongEventFormatException(string? message) : base(message)
        {
        }

        public WrongEventFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WrongEventFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}