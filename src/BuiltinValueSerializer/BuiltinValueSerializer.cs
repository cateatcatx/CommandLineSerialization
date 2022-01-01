using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinValueSerializer : IValueSerializer
    {
        private readonly Dictionary<Type, IValueSerializer> mSerializers;

        public BuiltinValueSerializer()
        {
            mSerializers = new Dictionary<Type, IValueSerializer>()
            {
                {typeof(int), new BuiltinIntSerializer()},
            };
        }
        
        public object? DeserializeNoneValue(Type objType)
        {
            if (!mSerializers.TryGetValue(objType, out var serializer))
            {
                throw _NewCantException(objType);
            }

            return serializer.DeserializeNoneValue(objType);
        }

        public object? DeserializeSingleValue(Type objType, string? value)
        {
            if (!mSerializers.TryGetValue(objType, out var serializer))
            {
                throw _NewCantException(objType);
            }

            return serializer.DeserializeSingleValue(objType, value);
        }

        public object? DeserializeMultiValue(Type objType, List<string> values)
        {
            if (!mSerializers.TryGetValue(objType, out var serializer))
            {
                throw _NewCantException(objType);
            }

            return serializer.DeserializeMultiValue(objType, values);
        }

        private InvalidOperationException _NewCantException(Type objType)
        {
            return new($"{nameof(BuiltinValueSerializer)} can not serialize or deserialize {objType}.");
        }
    }
}