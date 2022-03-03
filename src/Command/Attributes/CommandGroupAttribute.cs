using System;

namespace Decoherence.CommandLineSerialization
{
    public class CommandGroupAttribute : Attribute
    {
        public string? Group;

        public CommandGroupAttribute(string group)
        {
            Group = group;
        }
    }
}