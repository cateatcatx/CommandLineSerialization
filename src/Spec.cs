using System;

namespace Decoherence.CommandLineParsing
{
    public abstract class Spec
    {
        public bool Required { get; }
        
        public Func<object?>? DefaultValueCreator { get; }
        
        public abstract string Name { get; }
    }
}