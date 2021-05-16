using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decoherence.CommandLineParsing
{
    public delegate string SerializeFunc(CommandLineSerializer serializer, Type valueType, object? value);
    public delegate object? DeserializeFunc(CommandLineSerializer serializer, Type valueType, IEnumerable<string> args);
    
    public class SerializationStrategy
    {
        public SerializeFunc GetSerializeFunc(Type valueType)
        {
            throw new NotImplementedException();
        }

        public DeserializeFunc GetDeserializeFunc(Type valueType)
        {
            if (valueType.IsPrimitive)
            {
                return DeserializePrimitive;
            }
            else if (valueType.IsEnum)
            {
                return DeserializeEnum;
            }
        }

        private object? DeserializePrimitive(CommandLineSerializer serializer, Type valueType, IEnumerable<string> args)
        {
            var arg = args.First();
            return Convert.ChangeType(arg, valueType);
        }
        
        /// <summary>
        /// 例如 enum { A = 1, B = 100 }
        /// 支持如下输入：
        ///     A 或 1
        ///     A,B 或 A, B 或 1,100 或 1,B
        /// </summary>
        private object? DeserializeEnum(CommandLineSerializer serializer, Type valueType, IEnumerable<string> args)
        {
            var arg = args.First();
            var finalValue = 0;
            
            foreach (var item in arg.Split(','))
            {
                var itemTrimmed = item.Trim();
                if (string.IsNullOrWhiteSpace(itemTrimmed))
                {
                    continue;;
                }

                int? value = null;
                try
                {
                    value = (int)Enum.Parse(valueType, itemTrimmed);
                }
                catch
                {
                    // ignored
                }

                if (value == null && int.TryParse(itemTrimmed, out var enumValue))
                {
                    value = (int)Enum.ToObject(valueType, enumValue);
                }

                if (value == null)
                {
                    throw new InvalidArgsException($"'{arg}' is not the correct enum format.");
                }

                finalValue |= value.Value;
            }

            return finalValue;
        }
    }
}