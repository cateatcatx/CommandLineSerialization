using System;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable ReturnTypeCanBeNotNullable

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinListSerializer : IValueSerializer
    {
        public bool CanHandleType(Type objType)
        {
            return typeof(IList).IsAssignableFrom(objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            if (value is null)
                return null;
            
            return string.IsNullOrWhiteSpace(value) 
                ? null 
                : _Deserialize(objType, value.Split(','));
        }

        public object? DeserializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, LinkedList<string> argList)
        {
            return _Deserialize(objType, new List<string>(argList));
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            return _Deserialize(objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            return string.Join(",", SerializeSplitedSingleValue(serializer, objType, obj));
        }

        public LinkedList<string> SerializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            var argList = new LinkedList<string>();
            if (obj is IList list)
            {
                foreach (var item in list)
                {
                    argList.AddLast(item.ToString());
                }
            }

            return argList;
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            return SerializeSplitedSingleValue(serializer, objType, obj);
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