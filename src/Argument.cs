using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class Argument : Spec, IArgument
    {
        public Argument(ValueType valueType, Type objType, IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            if (!ImplUtils.IsValidArgumentValueType(valueType))
                throw new ArgumentException(ImplUtils.InvalidArgumentValueTypeError(valueType), nameof(valueType));
        }
    }
}