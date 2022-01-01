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

        public object? DeserializeNonValue(Type objType)
        {
            return null;
        }

        public object? DeserializeSingleValue(Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? null 
                : _Deserialize(objType, value.Split(','));
        }

        public object? DeserializeMultiValue(Type objType, List<string> values)
        {
            return _Deserialize(objType, values);
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