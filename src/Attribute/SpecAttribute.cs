using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public abstract class SpecAttribute : Attribute
    {
        public IValueSerializer? Serializer => 
            ValueSerializerType == null ? null : (IValueSerializer)Activator.CreateInstance(ValueSerializerType);

        public Type? ValueSerializerType
        {
            get => mValueSerializerType;
            set
            {
                if (value != null && !ImplUtils.IsValidValueSerializerType(value))
                    throw new ArgumentException(ImplUtils.InvalidValueSerializerTypeError(value));
                mValueSerializerType = value;
            }
        }

        public abstract ValueType ValueType
        {
            get;
            set;
        }

        private Type? mValueSerializerType;
        protected ValueType mValueType;
    }
}