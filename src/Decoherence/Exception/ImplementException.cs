using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Decoherence
{
    /// <summary>
    /// 实现错误
    /// </summary>
    public class ImplementException : Exception
    {
        public ImplementException()
        {
        }

        public ImplementException(string message) : base(message)
        {
        }

        public ImplementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImplementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
