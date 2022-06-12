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
                if (value != null && !DebugUtil.IsValidValueSerializerType(value))
                    throw new ArgumentException(DebugUtil.InvalidValueSerializerTypeError(value));
                mValueSerializerType = value;
            }
        }

        public string? ValueName
        {
            get;
            set;
        }

        public abstract ValueType ValueType
        {
            get;
            set;
        }

        public string? Desc
        {
            get;
            set;
        }

        private Type? mValueSerializerType;
    }
}