using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineApp : ICommandLineApp
    {
        public Command RootCommand { get; }

        public CommandLineApp(string name, string? desc)
        {
            RootCommand = new Command(name, desc);
        }
    }
}