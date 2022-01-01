using System;
using System.Reflection;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class ImplHelper
    {
        public static bool IsValidOptionLongName(string longName)
        {
            foreach (var ch in longName)
            {
                // 合法：字母、数字和-
                if (!ch.IsAlpha() && !ch.IsDigit() && ch != '-')
                    return false;
            }

            return true;
        }

        public static bool IsValidOptionShortName(char shortName)
        {
            return shortName.IsAlpha() || shortName.IsDigit();
        }

        public static bool IsValidValueSerializerType(Type valueSerializerType)
        {
            return typeof(IValueSerializer).IsAssignableFrom(valueSerializerType) && valueSerializerType.HasParameterlessConstructor();
        }

        public static ArgumentException NewInvalidOptionLongNameException(string longName, string paramName)
        {
            return new($"'{longName}' is not a valid long option name.", paramName);
        }
        
        public static ArgumentException NewInvalidOptionShortNameException(char shortName, string paramName)
        {
            return new($"'{shortName}' is not a valid short option name.", paramName);
        }

        public static ArgumentException NewInvalidValueSerializerTypeException(Type valueSerializerType, string paramName)
        {
            return new($"Type '{valueSerializerType}' is not derived from {nameof(IValueSerializer)} or has no constructor to invoke.", paramName);
        }
    }
}