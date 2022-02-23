using System;
using System.Runtime.Serialization;

namespace Decoherence
{
    public class SomethingWrongException : Exception
    {
        public SomethingWrongException()
        {
        }

        protected SomethingWrongException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SomethingWrongException(string message) : base(message)
        {
        }

        public SomethingWrongException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}