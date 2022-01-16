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

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.DeserializeNonValue(serializer, objType, matched);
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeSingleValue(serializer, objType, value);
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeMultiValue(serializer, objType, values);
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
            return new("No specified ValueSerializer.");
        }
    }
}