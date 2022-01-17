using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public abstract class Spec : ISpec
    {
        private const string InvalidOperationError = "No specified ValueSerializer.";
        
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

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);

            return mValueSerializer.GetObjectSpecs(objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);
            
            return mValueSerializer.DeserializeNonValue(serializer, objType, matched);
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);

            return mValueSerializer.DeserializeSingleValue(serializer, objType, value);
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);

            return mValueSerializer.DeserializeMultiValue(serializer, objType, values);
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);
            
            return mValueSerializer.SerializeNonValue(serializer, objType, obj);
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);
            
            return mValueSerializer.SerializeSingleValue(serializer, objType, obj);
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (mValueSerializer == null)
                throw new InvalidOperationException(InvalidOperationError);
            
            return mValueSerializer.SerializeMultiValue(serializer, objType, obj);
        }
    }
}