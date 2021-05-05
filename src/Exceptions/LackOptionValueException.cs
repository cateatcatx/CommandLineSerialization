using System;

namespace Decoherence.CommandLineParsing
{
    public class LackOptionValueException : Exception
    {
        public LackOptionValueException(string message) : base(message)
        {
        }
    }
}