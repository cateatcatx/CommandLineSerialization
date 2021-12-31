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
            foreach (var ch in longName)
            {
                // 合法：字母、数字和-
                if (!ch.IsAlpha() && !ch.IsDigit() && ch != '-')
                    throw new ArgumentException($"'{longName}' is not a valid long option name.", nameof(longName));
            }
            
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
            if (!shortName.IsAlpha() && !shortName.IsDigit())
                throw new ArgumentException($"'{shortName}' is not a valid short option name.", nameof(shortName));
            
            Name = shortName.ToString();
            ValueType = valueType;
        }
    }
}