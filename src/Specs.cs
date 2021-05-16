using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    public class Specs
    {
        public IEnumerable<Argument> Arguments => mArguments;
        public IEnumerable<Option> Options => mOptions;

        private readonly List<Argument> mArguments = new();
        private readonly List<Option> mOptions = new();
        
        public Argument AddArgument(Argument argument)
        {
            mArguments.Add(argument);
            return argument;
        }
        
        public Option AddOption(Option option)
        {
            if (option.ShortName != null && GetOptionByShortName(option.ShortName.Value) != null)
            {
                throw new ArgumentException($"Option with short name '-{option.ShortName}' already exists.");
            }
            if (option.LongName != null && GetOptionByLongName(option.LongName) != null)
            {
                throw new ArgumentException($"Option with long name '-{option.LongName}' already exists.");
            }
            
            mOptions.Add(option);
            return option;
        }
        
        public Argument? GetArgument(int position)
        {
            if (position < 0 || position >= mArguments.Count)
            {
                return null;
            }

            return mArguments[position];
        }

        public Option? GetOptionByShortName(char shortName)
        {
            return mOptions.Find(option => option.ShortName == shortName);
        }
        
        public Option? GetOptionByLongName(string longName)
        {
            return mOptions.Find(option => option.LongName == longName);
        }
    }
}