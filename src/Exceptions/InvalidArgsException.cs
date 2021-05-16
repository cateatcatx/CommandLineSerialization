using System;

namespace Decoherence.CommandLineParsing
{
    public class InvalidArgsException : Exception
    {
        public InvalidArgsException(string message) : base(message)
        {
        }
    }
}