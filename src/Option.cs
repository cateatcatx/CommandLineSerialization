using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public enum OptionType
    {
        Switch,
        Scalar,
        Sequence,
    }
    
    public class Option : Spec
    {
        public static Option NewRequired(Type valueType,
            OptionType type, 
            string? longName = null, 
            char? shortName = null,
            SerializeFunc? customSerializeFunc = null, 
            DeserializeFunc? customDeserializeFunc = null)
        {
            if (string.IsNullOrWhiteSpace(longName) && shortName == null)
                throw new ArgumentException($"Specify at least one of both {nameof(longName)} and {nameof(shortName)}");

            return new Option(valueType, type, longName, shortName, true, null, customSerializeFunc, customDeserializeFunc);
        }
        
        public static Option NewOptional(Type valueType,
            OptionType type, 
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

            return new Option(valueType, type, longName, shortName, false, defaultValueCreator, customSerializeFunc, customDeserializeFunc);
        }

        public OptionType Type { get; }

        public string? LongName { get; }
        
        public char? ShortName { get; }
        
        public override string Name => LongName ?? ShortName.ToString();
        
        

        protected Option(Type valueType, 
            OptionType type, 
            string? longName, 
            char? shortName, 
            bool required, 
            Func<object?>? defaultValueCreator,
            SerializeFunc? customSerializeFunc,
            DeserializeFunc? customDeserializeFunc)
            : base(valueType, required, defaultValueCreator, customSerializeFunc, customDeserializeFunc)
        {
            Type = type;
            LongName = longName;
            ShortName = shortName;
        }
    }
}