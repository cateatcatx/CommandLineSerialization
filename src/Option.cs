using System;
using System.Collections.Generic;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Option : Spec, IOption
    {
        public string Name { get; }
        public OptionValueType ValueType { get; }

        public Option(
            string longName, 
            OptionValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(objType, valueSerializer)
        {
            if (!ImplHelper.IsValidOptionLongName(longName))
                throw ImplHelper.NewInvalidOptionLongNameException(longName, nameof(longName));
            
            Name = longName;
            ValueType = valueType;
        }
        
        public Option(
            char shortName, 
            OptionValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(objType, valueSerializer)
        {
            if (!ImplHelper.IsValidOptionShortName(shortName))
                throw ImplHelper.NewInvalidOptionShortNameException(shortName, nameof(shortName));
            
            Name = shortName.ToString();
            ValueType = valueType;
        }
    }
}