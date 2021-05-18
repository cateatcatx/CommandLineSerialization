using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decoherence.CommandLineParsing
{
    public class CommandLineSerializer
    {
        private readonly SerializationStrategy mSerializationStrategy;

        public CommandLineSerializer(SerializationStrategy? serializationStrategy = null)
        {
            mSerializationStrategy = serializationStrategy ?? new SerializationStrategy();
        }
        
        public void DeserializeValues(IEnumerable<string> args, Specs specs, out Values values, out LinkedList<string> remainArgs)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (specs == null)
                throw new ArgumentNullException(nameof(specs));

            values = new Values();
            remainArgs = new LinkedList<string>(args);
            List<string> matchedArgs = new();

            // --- 先解析Option ---
            List<Option> options = new(specs.Options);
            
            // 把Switch排在最前面，这样可以保证在处理ShortName时先消耗掉Switch，这样合法的Scalar和Sequence就在最后了
            options.Sort((a, b) => (int)a.Type - (int)b.Type);
            
            foreach (var option in options)
            {
                object? value;
                
                matchedArgs.Clear();
                var matched = _MatchOptionAndConsumeArgs(option, remainArgs, matchedArgs);
                if (!matched)
                {
                    if (option.Required)
                    {
                        throw new InvalidArgsException($"Lack of required option '{option.Name}'.");
                    }

                    value = option.DefaultValueCreator!();
                }
                else
                {
                    value = DeserializeValue(option.ValueType, matchedArgs);
                }
                
                values.AddValue(option, value);
            }
            
            // 再解析Argument
            foreach (var argument in specs.Arguments)
            {
                object? value;
                var first = remainArgs.First;

                if (first == null)
                {
                    if (argument.Required)
                    {
                        throw new InvalidArgsException($"Lack of required argument '{argument.Name}'.");
                    }

                    value = argument.DefaultValueCreator!();
                }
                else
                {
                    value = DeserializeValue(argument.ValueType, remainArgs);
                    remainArgs.RemoveFirst();
                }
                
                values.AddValue(argument, value);
            }
        }

        public string SerializeValue(Type valueType, object? value)
        {
            throw new NotImplementedException();
        }

        public object? DeserializeValue(Type valueType, IEnumerable<string> args)
        {
            var func = mSerializationStrategy.GetDeserializeFunc(valueType);
            return func(this, valueType, args);
        }

        private bool _MatchOptionAndConsumeArgs(Option option, LinkedList<string> argList, List<string> matchedArgs)
        {
            // LongName比ShortName更具体，所以先匹配LongName
            if (option.LongName != null)
            {
                var node = argList.Find($"--{option.LongName}");
                if (node != null)
                {
                    _HandleOption(option, argList, matchedArgs, node, -1);
                    return true;
                }
            }
            
            if (option.ShortName != null)
            {
                var shortNameIndex = -1;
                LinkedListNode<string>? node = null;
                var tmpNode = argList.First;
                while (tmpNode != null)
                {
                    if (tmpNode.Value.StartsWith("-"))
                    {
                        shortNameIndex = tmpNode.Value.IndexOf(option.ShortName.Value);
                        if (shortNameIndex > 0)
                        {
                            node = tmpNode;
                            break;
                        }
                    }
                    
                    tmpNode = tmpNode.Next;
                }

                if (node == null)
                {
                    return false;
                }

                _HandleOption(option, argList, matchedArgs, node, shortNameIndex);
                return true;
            }

            return false;
        }

        private void _HandleOption(Option option, LinkedList<string> argList, List<string> matchedArgs, LinkedListNode<string> optionNode, int shortNameIndex)
        {
            if (option.Type == OptionType.Switch)
            {
                if (shortNameIndex == -1)
                {
                    argList.Remove(optionNode);
                }
                else
                {
                    var newValue = optionNode.Value.Remove(shortNameIndex, 1);
                    if (newValue == "-")
                    {
                        argList.Remove(optionNode);
                    }
                    else
                    {
                        optionNode.Value = newValue;
                    }
                }
                
                return;
            }
            
            var optionName = shortNameIndex == -1 ? $"--{option.LongName}" : $"-{option.ShortName}";

            if (option.Type == OptionType.Scalar)
            {
                string? optionValue = null;
                if (shortNameIndex != -1 && shortNameIndex < optionNode.Value.Length - 1)
                {
                    optionValue = optionNode.Value.Substring(shortNameIndex + 1);
                    argList.Remove(optionNode);
                }
                else
                {
                    if (!_IsValidOptionValueNode(optionNode.Next))
                    {
                        throw new InvalidArgsException($"Lack value of option '{optionName}'.");
                    }

                    optionValue = optionNode.Next!.Value;
                    argList.Remove(optionNode.Next);
                    argList.Remove(optionNode);
                }

                matchedArgs.Add(optionValue);
            }
            else if (option.Type == OptionType.Sequence)
            {
                string? optionValue = null;
                LinkedListNode<string>? node = null;
                if (shortNameIndex != -1 && shortNameIndex < optionNode.Value.Length - 1)
                {
                    optionValue = optionNode.Value.Substring(shortNameIndex + 1);
                    node = optionNode;
                }
                else if (_IsValidOptionValueNode(optionNode.Next))
                {
                    optionValue = optionNode.Next!.Value;
                    node = optionNode.Next;
                    argList.Remove(optionNode);
                }
                
                while (optionValue != null)
                {
                    matchedArgs.Add(optionValue);

                    if (!_IsValidOptionValueNode(node!.Next))
                    {
                        continue;
                    }
                    
                    var tmpNode = node.Next;
                    argList.Remove(node);
                    node = tmpNode;
                    optionValue = node!.Value;
                }
            }
        }

        private bool _IsValidOptionValueNode(LinkedListNode<string>? node)
        {
            // "--"是option结束符
            return node != null && node.Value != "--";
        }
    }
}