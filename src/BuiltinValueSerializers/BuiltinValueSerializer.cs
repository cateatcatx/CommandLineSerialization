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
            BuiltinStringSerializer builtinStringSerializer = new();
            BuiltinBoolSerializer builtinBoolSerializer = new();
            
            mSerializer = new List<IValueSerializer>
            {
                builtinIntSerializer,
                builtinStringSerializer,
                builtinBoolSerializer,
                new BuiltinListSerializer(),
                new BuiltinEnumSerializer(),
                new BuiltinObjectSerializer(), // Object最好放在最后
            };
            
            // 预cache
            mType2Serializer = new Dictionary<Type, IValueSerializer>
            {
                {typeof(int), builtinIntSerializer},
                {typeof(string), builtinStringSerializer},
                {typeof(bool), builtinBoolSerializer},
            };
        }

        public bool CanHandleType(Type objType)
        {
            throw new NotImplementedException();
        }

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.GetObjectSpecs(objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.DeserializeNonValue(serializer, objType, matched);
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.DeserializeSingleValue(serializer, objType, value);
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.DeserializeMultiValue(serializer, objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.SerializeNonValue(serializer, objType, obj);
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.SerializeSingleValue(serializer, objType, obj);
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (!_TryGetSerializer(objType, out var serial))
            {
                throw new InvalidOperationException(_GenCantSerializeErr(objType));
            }

            return serial.SerializeMultiValue(serializer, objType, obj);
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