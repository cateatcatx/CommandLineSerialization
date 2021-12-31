using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpecs
    {
        IReadOnlyDictionary<string, IOption> Options { get; }
        IReadOnlyList<IArgument> Arguments { get; }
    }
}