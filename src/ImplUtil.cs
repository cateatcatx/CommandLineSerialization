using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization
{
    public static class ImplUtil
    {
        public static ISpec GenerateSpecByAttribute(
            SpecAttribute attr, 
            Type objType, 
            string defaultOptionName,
            ValueType defaultValueType,
            IValueSerializer? defaultValueSerializer)
        {
            if (attr is ArgumentAttribute argumentAttr)
            {
                return new Argument(
                    argumentAttr.Priority,
                    argumentAttr.ValueType != ValueType.Default ? argumentAttr.ValueType : defaultValueType, 
                    objType, 
                    argumentAttr.Serializer ?? defaultValueSerializer);
            }

            var optionAttr = (OptionAttribute)attr;
            return new Option(
                optionAttr.Name ?? defaultOptionName, 
                optionAttr.ValueType != ValueType.Default ? optionAttr.ValueType : defaultValueType, 
                objType, 
                optionAttr.Serializer ?? defaultValueSerializer);
        }
    }
}