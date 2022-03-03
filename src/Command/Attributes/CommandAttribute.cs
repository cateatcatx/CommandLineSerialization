using System;

namespace Decoherence.CommandLineSerialization
{
    public class CommandAttribute : Attribute
    {
        public string? Name;
        public string? Desc;
    }
}