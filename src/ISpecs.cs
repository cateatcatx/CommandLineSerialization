using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpecs
    {
        IReadOnlyList<IOption> Options { get; }
        IReadOnlyList<IArgument> Arguments { get; }
    }
}