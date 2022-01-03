using System;
using System.Collections.Generic;
using System.Data;

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineSerializer
    {
        public delegate object? OnSerialize(ISpec spec);

        private readonly IValueSerializer mValueSerializer;

        public CommandLineSerializer(IValueSerializer? valueSerializer = null)
        {
            mValueSerializer = valueSerializer ?? new BuiltinIntSerializer();
        }

        public LinkedList<string> Serialize(ISpecs specs, OnSerialize? onSerialize)
        {
            LinkedList<string> argList = new();
            _SerializeOptions(specs.Options, argList, onSerialize);
            _SerializeArguments(specs.Arguments, argList, onSerialize);
            return argList;
        }

        private void _SerializeOptions(IReadOnlyDictionary<string, IOption> options, LinkedList<string> argList, OnSerialize? onSerialize)
        {
            foreach (var kv in options)
            {
                var option = kv.Value;
                var obj = onSerialize?.Invoke(option);
                var optionPrefix = option.Name.Length > 1 ? $"--{option.Name}" : $"-{option.Name}";
                
                if (option.ValueType == ValueType.Non)
                {
                    if (mValueSerializer.SerializeNonValue(this, option.ObjType, obj))
                    {
                        argList.AddLast(optionPrefix);
                    }
                    
                    continue;
                }

                if (option.ValueType == ValueType.Single)
                {
                    argList.AddLast($"{optionPrefix}");
                    argList.AddLast(mValueSerializer.SerializeSingleValue(this, option.ObjType, obj));
                    
                    continue;
                }
                
                if (option.ValueType == ValueType.Multi)
                {
                    foreach (string value in mValueSerializer.SerializeMultiValue(this, option.ObjType, obj))
                    {
                        argList.AddLast($"{optionPrefix}");
                        argList.AddLast(value);
                    }
                    
                    continue;
                }
                
                if (option.ValueType == ValueType.Sequence)
                {
                    argList.AddLast($"{optionPrefix}");
                    foreach (string value in mValueSerializer.SerializeMultiValue(this, option.ObjType, obj))
                    {
                        argList.AddLast(value);
                    }
                    
                    continue;
                }
            }
        }

        private void _SerializeArguments(IReadOnlyList<IArgument> arguments, LinkedList<string> argList, OnSerialize? onSerialize)
        {
            foreach (var argument in arguments)
            {
                var obj = onSerialize?.Invoke(argument);
                
                if (argument.ValueType == ValueType.Single)
                {
                    argList.AddLast(mValueSerializer.SerializeSingleValue(this, argument.ObjType, obj));
                    
                    continue;
                }

                if (argument.ValueType == ValueType.Sequence)
                {
                    foreach (string value in mValueSerializer.SerializeMultiValue(this, argument.ObjType, obj))
                    {
                        argList.AddLast(value);
                    }
                    
                    continue;
                }
            }
        }
    }
}