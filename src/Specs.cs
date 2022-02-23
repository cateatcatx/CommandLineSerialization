using System;
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Specs : ISpecs
    {
        public IReadOnlyDictionary<string, IOption> Options => mOptions;
        public IReadOnlyList<IArgument> Arguments => mArguments;

        private readonly Dictionary<string, IOption> mOptions = new();
        private readonly PriorityList<IArgument> mArguments = new((a, b) => b.Priority - a.Priority);
        
        public void AddOption(IOption option)
        {
            if (option.ShortName != null && !mOptions.TryAdd(option.ShortName.ToString(), option))
            {
                throw new ArgumentException($"Option with short name '{option.ShortName}' already exists.");
            }
            
            if (option.LongName != null && !mOptions.TryAdd(option.LongName, option))
            {
                throw new ArgumentException($"Option with long name '{option.LongName}' already exists.");
            }
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