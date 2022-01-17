using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable ReturnTypeCanBeNotNullable

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinListSerializer : IValueSerializer
    {
        private IValueSerializer? mValueSerializer;
        private IValueSerializer ValueSerializer => mValueSerializer ??= new BuiltinValueSerializer();
        
        public bool CanHandleType(Type objType)
        {
            return typeof(IList).IsAssignableFrom(objType);
        }

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Sequence, objType), objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? null 
                : _Deserialize(serializer, objType, value.Split(','));
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            return _Deserialize(serializer, objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            return string.Join(",", SerializeMultiValue(serializer, objType, obj));
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            var argList = new LinkedList<string>();
            if (obj is IList list)
            {
                foreach (var item in list)
                {
                    argList.AddLast(ImplUtil.MergeCommandLine(serializer.SerializeObject(item)));
                }
            }

            return argList;
        }

        private object? _Deserialize(CommandLineSerializer serializer, Type objType, IList<string> values)
        {
            Type? itemType = ReflectUtil.GetListItemType(objType);
            IList list = ReflectUtil.CreateList(objType, values.Count);
            for (var i = 0; i < values.Count; ++i)
            {
                var v = values[i];
                object? obj = v;
                if (itemType != null)
                {
                    obj = ValueSerializer.DeserializeSingleValue(serializer, itemType, v);
                }
                
                _AddValueToList(list, i, obj);
            }

            return list;
        }

        private void _AddValueToList(IList list, int index, object? value)
        {
            if (list is Array array)
            {
                array.SetValue(value, index);
            }
            else
            {
                list.Add(value);
            }
        }
    }
}