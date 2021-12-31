using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{

    public class Option : Spec
    {
        public static Option NewRequired(Type valueType,
            OptionValueType valueType, 
            string? longName = null, 
            char? shortName = null,
            SerializeFunc? customSerializeFunc = null, 
            DeserializeFunc? customDeserializeFunc = null)
        {
            if (string.IsNullOrWhiteSpace(longName) && shortName == null)
                throw new ArgumentException($"Specify at least one of both {nameof(longName)} and {nameof(shortName)}");

            return new Option(valueType, valueType, longName, shortName, true, null, customSerializeFunc, customDeserializeFunc);
        }
        
        public static Option NewOptional(Type valueType,
            OptionValueType valueType, 
            Func<object?> defaultValueCreator, 
            string? longName = null, 
            char? shortName = null,
            SerializeFunc? customSerializeFunc = null, 
            DeserializeFunc? customDeserializeFunc = null)
        {
            if (defaultValueCreator is null)
                throw new ArgumentNullException(nameof(defaultValueCreator));
            if (string.IsNullOrWhiteSpace(longName) && shortName == null)
                throw new ArgumentException($"Specify at least one of both {nameof(longName)} and {nameof(shortName)}");

            return new Option(valueType, valueType, longName, shortName, false, defaultValueCreator, customSerializeFunc, customDeserializeFunc);
        }

        public OptionValueType ValueType { get; }

        public string? LongName { get; }
        
        public char? ShortName { get; }
        
        public override string Name => LongName ?? ShortName.ToString();
        
        

        protected Option(Type valueType, 
            OptionValueType valueType, 
            string? longName, 
            char? shortName, 
            bool required, 
            Func<object?>? defaultValueCreator,
            SerializeFunc? customSerializeFunc,
            DeserializeFunc? customDeserializeFunc)
            : base(valueType, required, defaultValueCreator, customSerializeFunc, customDeserializeFunc)
        {
            ValueType = valueType;
            LongName = longName;
            ShortName = shortName;
        }
    }
}