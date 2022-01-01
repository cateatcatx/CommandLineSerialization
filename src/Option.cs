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
            string name,
            OptionValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(objType, valueSerializer)
        {
            if (name.Length > 1)
            {
                if (!ImplHelper.IsValidOptionLongName(name))
                    throw ImplHelper.NewInvalidOptionLongNameException(name, nameof(name));
            }
            else
            {
                if (!ImplHelper.IsValidOptionShortName(name))
                    throw ImplHelper.NewInvalidOptionShortNameException(name, nameof(name));
            }
            
            Name = name;
            ValueType = valueType;
        }
    }
}