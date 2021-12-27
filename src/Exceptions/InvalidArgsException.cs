using System;

namespace Decoherence.CommandLineSerialization
{
    public class InvalidArgsException : Exception
    {
        public InvalidArgsException(string message) : base(message)
        {
        }
    }
}