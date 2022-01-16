using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpecs
    {
        IReadOnlyDictionary<string, IOption> Options { get; }
        IReadOnlyList<IArgument> Arguments { get; }
    }
}