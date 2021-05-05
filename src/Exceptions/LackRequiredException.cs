using System;

namespace Decoherence.CommandLineParsing
{
    public class LackRequiredException : Exception
    {
        public LackRequiredException(string message) : base(message)
        {
        }
    }
}