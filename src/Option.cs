using System;
using System.Collections.Generic;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class Option : Spec, IOption
    {
        public string Name { get; }

        public Option(
            string name,
            ValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            if (name.Length > 1)
            {
                if (!ImplUtils.IsValidOptionLongName(name))
                    throw new ArgumentException(ImplUtils.InvalidOptionLongNameError(name), nameof(name));
            }
            else
            {
                if (!ImplUtils.IsValidOptionShortName(name))
                    throw new ArgumentException(ImplUtils.InvalidOptionShortNameError(name), nameof(name));
            }

            if (!ImplUtils.IsValidOptionValueType(valueType))
                throw new ArgumentException(ImplUtils.InvalidOptionValueTypeError(valueType), nameof(valueType));
            
            Name = name;
        }
    }
}