using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public abstract class Spec
    {
        public abstract string Name { get; }
        
        public Type ValueType { get; }
        
        public bool Required { get; }
        
        public Func<object?>? DefaultValueCreator { get; }
        
        public SerializeFunc? CustomSerializeFunc { get; }
        
        public DeserializeFunc? CustomDeserializeFunc { get; }

        protected Spec(Type valueType, 
            bool required, 
            Func<object?>? defaultValueCreator, 
            SerializeFunc? customSerializeFunc,
            DeserializeFunc? customDeserializeFunc)
        {
            if (valueType is null)
                throw new ArgumentNullException(nameof(valueType));
            if (!required && defaultValueCreator is null)
                throw new ArgumentNullException(nameof(defaultValueCreator), $"Optional value must specify {nameof(defaultValueCreator)}.");
                
            ValueType = valueType;
            Required = required;
            DefaultValueCreator = defaultValueCreator;
            CustomSerializeFunc = customSerializeFunc;
            CustomDeserializeFunc = customDeserializeFunc;
        }
        
        public string SerializeValue(CommandLineSerializer serializer, object? value)
        {
            return CustomSerializeFunc?.Invoke(serializer, ValueType, value) ?? serializer.SerializeValue(ValueType, value);
        }

        public object? DeserializeValue(CommandLineSerializer serializer, IEnumerable<string> args)
        {
            return CustomDeserializeFunc?.Invoke(serializer, ValueType, args) ?? serializer.DeserializeValue(ValueType, args);
        }
    }
}