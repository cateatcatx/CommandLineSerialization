using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Decoherence;

public class PathNotFoundException : Exception
{
    public PathNotFoundException()
    {
    }

    public PathNotFoundException(string message) : base(message)
    {
    }

    public PathNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}