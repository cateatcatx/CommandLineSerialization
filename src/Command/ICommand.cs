using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization;

public interface ICommand : ISpecs
{
    event Action? BeforeAllMethod;
    event Action? AfterAllMethod;
    event Action<MethodBase, ParameterInfo[], object?[]>? BeforeOneMethod;
    event Action<MethodBase, object?>? AfterOneMethod;
        
    string Name { get; }
    string? Desc { get; }
        
    IEnumerable<ICommand> Subcommands { get; }
    IEnumerable<string> Groups { get; }
    IReadOnlyDictionary<string, IEnumerable<ICommand>> Group2Subcommands { get; }

    int? Run(LinkedList<string> argList);
}
    
public interface ICommand<out T> : ICommand
    where T : ICommand
{
    new IEnumerable<T> Subcommands { get; }
}