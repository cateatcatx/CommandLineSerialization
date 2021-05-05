using System;
using System.Collections;
using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    public class Specs
    {
        public IEnumerable<Argument> Arguments { get; }
        
        public IEnumerable<Option> Options { get; }
        
        public Argument AddArgument(Argument argument)
        {
            throw new NotImplementedException();
        }
        
        public Option AddOption(Option option)
        {
            throw new NotImplementedException();
        }
        
        public Argument GetArgument(int position)
        {
            throw new NotImplementedException();
        }

        public Option GetOptionByShortName(string shortName)
        {
            throw new NotImplementedException();
        }
        
        public Option GetOptionByLongName(string longName)
        {
            throw new NotImplementedException();
        }
    }
}