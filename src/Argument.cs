using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class Argument : Spec, IArgument
    {
        public ArgumentValueType ValueType { get; }

        public Argument(ArgumentValueType valueType, Type objType, IValueSerializer? valueSerializer = null)
            : base(objType, valueSerializer)
        {
            ValueType = valueType;
        }
    }
}