using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    public enum ArgumentType
    {
        Scalar,
        Sequence,
    }
    
    public class Argument : Spec
    {
        public static Argument NewRequired(Type valueType, 
            ArgumentType type, 
            string name, 
            SerializeFunc? customSerializeFunc = null, 
            DeserializeFunc? customDeserializeFunc = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            return new Argument(valueType, type, name, true, null, customSerializeFunc, customDeserializeFunc);
        }
        
        public static Argument NewOptional(Type valueType,
            ArgumentType type, 
            string name, 
            Func<object?> defaultValueCreator,
            SerializeFunc? customSerializeFunc = null, 
            DeserializeFunc? customDeserializeFunc = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));
            if (defaultValueCreator is null)
                throw new ArgumentNullException(nameof(defaultValueCreator));

            return new Argument(valueType, type, name, false, defaultValueCreator, customSerializeFunc, customDeserializeFunc);
        }
        
        public ArgumentType Type { get; }
        
        public override string Name { get; }
        

        protected Argument(Type valueType,
            ArgumentType type, 
            string name, 
            bool required, 
            Func<object?>? defaultValueCreator,
            SerializeFunc? customSerializeFunc,
            DeserializeFunc? customDeserializeFunc)
            : base(valueType, required, defaultValueCreator, customSerializeFunc, customDeserializeFunc)
        {
            Type = type;
            Name = name;
        }
    }
}