using System.Runtime.Serialization;

namespace Churro.Errors
{
    [Serializable]
    internal class RuntimeError : Exception
    {
        public Token @operator;
        public string v;

        public RuntimeError()
        {
            Console.WriteLine("ERROR");
        }

        public RuntimeError(string? message) : base(message)
        {
        }

        public RuntimeError(Token @operator, string v)
        {
            this.@operator = @operator;
            this.v = v;
        }

        public RuntimeError(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RuntimeError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}