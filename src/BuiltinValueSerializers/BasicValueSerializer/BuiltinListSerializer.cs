using System;
using System.Collections;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinListSerializer : IValueSerializer
    {
        public bool CanHandleType(Type objType)
        {
            return typeof(IList).IsAssignableFrom(objType);
        }

        public object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType)
        {
            return null;
        }

        public object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? null 
                : _Deserialize(objType, value.Split(','));
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            return _Deserialize(objType, new List<string>(argList));
        }

        public object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values)
        {
            return _Deserialize(objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }

        private static object? _Deserialize(Type objType, IList values)
        {
            Type? innerType = null;
            if (objType.HasElementType)
            {
                innerType = objType.GetElementType();
            }

            IList list = ReflectUtil.CreateList(objType, values.Count);
            for (var i = 0; i < values.Count; ++i)
            {
                object? v = values[i];
                if (innerType != null)
                {
                    v = Convert.ChangeType(values[i], innerType);
                }

                list[i] = v;
            }

            return list;
        }
    }
}