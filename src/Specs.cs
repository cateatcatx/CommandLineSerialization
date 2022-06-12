using System;
using System.Collections.Generic;
using System.Net.Sockets;
// ReSharper disable once RedundantUsingDirective
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Specs : ISpecs
    {
        public IReadOnlyList<IOption> Options => mOptions;
        public IReadOnlyList<IArgument> Arguments => mArguments;

        private readonly List<IOption> mOptions = new();
        private readonly PriorityList<IArgument> mArguments = new((a, b) => b.Priority - a.Priority);
        
        public void AddOption(IOption option)
        {
            if (option.ShortName != null && mOptions.FindIndex(item => item.ShortName == option.ShortName) >= 0)
            {
                throw new ArgumentException($"Option with short name '{option.ShortName}' already exists.");
            }
            
            if (option.LongName != null && mOptions.FindIndex(item => item.LongName == option.LongName) >= 0)
            {
                throw new ArgumentException($"Option with long name '{option.LongName}' already exists.");
            }
            
            mOptions.Add(option);
        }

        public void AddArgument(IArgument argument)
        {
            mArguments.Add(argument);
        }

        public void AddSpec(ISpec spec)
        {
            if (spec is IOption option)
            {
                AddOption(option);
            }
            else
            {
                AddArgument((IArgument)spec);
            }
        }
    }
}