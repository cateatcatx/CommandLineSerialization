using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public interface ICommandLineApp
    {
        event Action? AfterGlobalMethod;
        
        int Start(IEnumerable<string> args);
        void AddGlobalMethod(MethodBase method, Func<object?>? objGetter);
        void AddCommand(MethodBase method, Func<object?>? objGetter, string? commandName = null);
    }
}