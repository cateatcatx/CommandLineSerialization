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
                if (!DebugUtil.IsValidOptionLongName(name))
                    throw new ArgumentException(DebugUtil.InvalidOptionLongNameError(name), nameof(name));
            }
            else
            {
                if (!DebugUtil.IsValidOptionShortName(name))
                    throw new ArgumentException(DebugUtil.InvalidOptionShortNameError(name), nameof(name));
            }

            if (!DebugUtil.IsValidOptionValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidOptionValueTypeError(valueType), nameof(valueType));
            
            Name = name;
        }
    }
}