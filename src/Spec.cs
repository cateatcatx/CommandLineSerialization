using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public abstract class Spec : ISpec
    {
        public ValueType ValueType { get; }
        public Type ObjType { get; }

        protected readonly IValueSerializer? mValueSerializer;

        protected Spec(ValueType valueType, Type objType, IValueSerializer? valueSerializer)
        {
            ValueType = valueType;
            ObjType = objType;
            mValueSerializer = valueSerializer;
        }

        public bool CanHandleType(Type objType)
        {
            return mValueSerializer != null && objType == ObjType;
        }

        public object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.DeserializeNonValue(deserializer, objType);
        }

        public object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeSingleValue(deserializer, objType, value);
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeSplitedSingleValue(deserializer, objType, argList);
        }

        public object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeMultiValue(deserializer, objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.SerializeNonValue(serializer, objType, obj);
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.SerializeSingleValue(serializer, objType, obj);
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.SerializeMultiValue(serializer, objType, obj);
        }

        private InvalidOperationException _NewInvalidOperationException()
        {
            return new InvalidOperationException("No specified ValueSerializer.");
        }
    }
}