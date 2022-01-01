using System;
using System.Collections.Generic;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Specs : ISpecs
    {
        public IReadOnlyDictionary<string, IOption> Options => mOptions;
        public IReadOnlyList<IArgument> Arguments => mArguments;

        private readonly Dictionary<string, IOption> mOptions = new();
        private readonly List<IArgument> mArguments = new();

        public void AddOption(IOption option)
        {
            if (!mOptions.TryAdd(option.Name, option))
            {
                throw new ArgumentException($"Option with name '{option.Name}' already exists.");
            }
        }

        public void AddArgument(IArgument argument)
        {
            mArguments.Add(argument);
        }
    }
}