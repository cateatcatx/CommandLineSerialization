using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinValueSerializer : IValueSerializer
    {
        private readonly List<IValueSerializer> mSerializer;
        private readonly Dictionary<Type, IValueSerializer> mType2Serializer;

        public BuiltinValueSerializer()
        {
            BuiltinIntSerializer builtinIntSerializer = new();
            
            mSerializer = new List<IValueSerializer>()
            {
                builtinIntSerializer,
                new BuiltinListSerializer(),
                new BuiltinObjectSerializer(),
            };
            
            // 预cache
            mType2Serializer = new Dictionary<Type, IValueSerializer>()
            {
                {typeof(int), builtinIntSerializer},
            };
        }

        public bool CanHandleType(Type objType)
        {
            throw new NotImplementedException();
        }

        public object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType)
        {
            if (!_TryGetSerializer(objType, out var serializer))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serializer.DeserializeNonValue(deserializer, objType);
        }

        public object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value)
        {
            if (!_TryGetSerializer(objType, out var serializer))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serializer.DeserializeSingleValue(deserializer, objType, value);
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            if (!_TryGetSerializer(objType, out var serializer))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serializer.DeserializeSplitedSingleValue(deserializer, objType, argList);
        }

        public object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values)
        {
            if (!_TryGetSerializer(objType, out var serializer))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serializer.DeserializeMultiValue(deserializer, objType, values);
        }

        private bool _TryGetSerializer(Type objType, out IValueSerializer serializer)
        {
            if (mType2Serializer.TryGetValue(objType, out serializer))
            {
                return true;
            }

            foreach (var valueSerializer in mSerializer)
            {
                if (valueSerializer.CanHandleType(objType))
                {
                    mType2Serializer.Add(objType, valueSerializer);
                    serializer = valueSerializer;
                    return true;
                }
            }

            return false;
        }
        
        private string _GenCantSerializeErr(Type objType)
        {
            return $"{nameof(BuiltinValueSerializer)} can not serialize or deserialize {objType}.";
        }
    }
}