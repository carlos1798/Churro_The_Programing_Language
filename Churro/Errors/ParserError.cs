using System.Runtime.Serialization;

namespace Churro.Errors
{
    [Serializable]
    internal class ParserError : Exception
    {
        public ParserError()
        {
        }

        public ParserError(string? message) : base(message)
        {
        }

        public ParserError(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ParserError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}