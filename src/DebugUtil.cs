using System;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public static class DebugUtil
    {
        public static bool IsValidOptionValueType(ValueType valueType)
        {
            return true;
        }
        
        public static string InvalidOptionValueTypeError(ValueType valueType)
        {
            return "";
        }

        public static bool IsValidArgumentValueType(ValueType valueType)
        {
            return valueType is ValueType.Single or ValueType.Sequence;
        }
        
        public static string InvalidArgumentValueTypeError(ValueType valueType)
        {
            return $"{valueType} is a invalid argument value type, only can be {nameof(ValueType.Single)} and {nameof(ValueType.Sequence)}.";
        }
        
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
        
        public static string InvalidOptionLongNameError(string longName)
        {
            return $"'{longName}' is not a valid long option name.";
        }
        
        public static bool IsValidOptionShortName(string shortName)
        {
            return shortName.Length == 1 && (shortName.IsAlpha() || shortName.IsDigit());
        }
        
        public static string InvalidOptionShortNameError(string shortName)
        {
            return $"'{shortName}' is not a valid short option name.";
        }

        public static bool IsValidValueSerializerType(Type valueSerializerType)
        {
            return typeof(IValueSerializer).IsAssignableFrom(valueSerializerType) && valueSerializerType.HasParameterlessConstructor();
        }
        
        public static string InvalidValueSerializerTypeError(Type valueSerializerType)
        {
            return $"Type '{valueSerializerType}' is not derived from {nameof(IValueSerializer)} or has no constructor to invoke.";
        }

        
        
        

        

        
    }
}