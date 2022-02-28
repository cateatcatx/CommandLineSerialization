using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public interface ICommandLineApp
    {
        Command RootCommand { get; }
    }
}