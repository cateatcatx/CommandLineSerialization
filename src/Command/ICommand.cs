using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public interface ICommand
    {
        event Action? BeforeAllMethod;
        event Action? AfterAllMethod;
        event Action<MethodBase, ParameterInfo[], object?[]>? BeforeOneMethod;
        event Action<MethodBase, object?>? AfterOneMethod;
        
        string Name { get; }
        
        string? Desc { get; }
        
        int MaxLineLength { get; }
        
        IEnumerable<ICommand> Subcommands { get; }
        
        int? Run(LinkedList<string> argList);
        
        void Draw(CommandLineWriter writer);
    }
    
    public interface ICommand<out T> : ICommand
        where T : ICommand
    {
        new IEnumerable<T> Subcommands { get; }
    }
}