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

        public object? DeserializeNonValue(Type objType)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();
            
            return mValueSerializer.DeserializeNonValue(objType);
        }

        public object? DeserializeSingleValue(Type objType, string? value)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeSingleValue(objType, value);
        }

        public object? DeserializeMultiValue(Type objType, List<string> values)
        {
            if (mValueSerializer == null)
                throw _NewInvalidOperationException();

            return mValueSerializer.DeserializeMultiValue(objType, values);
        }

        private InvalidOperationException _NewInvalidOperationException()
        {
            return new InvalidOperationException("No specified ValueSerializer.");
        }
    }
}