using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decoherence.CommandLineParsing
{
    public class CommandLineSerializer
    {
        private readonly SerializationStrategy mSerializationStrategy;
        
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

                    value = option.Type == OptionType.Switch ? option.DeserializeSwitch(matched) : option.DefaultValueCreator!();
                }
                else
                {
                    if (option.Type == OptionType.Switch)
                    {
                        value = option.DeserializeSwitch(matched);
                    }
                    else if (option.Type == OptionType.Scalar)
                    {
                        value = option.DeserializeScalar(matchedArgs[0]);
                    }
                    else if (option.Type == OptionType.Sequence)
                    {
                        value = option.DeserializeSequence(matchedArgs);
                    }
                    else
                    {
                        throw new NotImplementedException(option.Type.ToString());
                    }
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
                    if (argument.Type == ArgumentType.Scalar)
                    {
                        value = argument.DeserializeScalar(first.Value);
                        remainArgs.RemoveFirst();
                    }
                    else if (argument.Type == ArgumentType.Sequence)
                    {
                        value = argument.DeserializeSequence(remainArgs);
                        while (remainArgs.First != null)
                        {
                            remainArgs.RemoveFirst();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(argument.Type.ToString());
                    }
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
             mSerializationStrategy.GetDeserializeFunc(valueType)
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
            else if (option.ShortName != null)
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

            throw new ImplException();
        }

        private void _HandleOption(Option option, LinkedList<string> argList, List<string> matchedArgs, LinkedListNode<string> optionNode, int shortNameIndex)
        {
            if (option.Type == OptionType.Switch)
            {
                _ConsumeOptionArg(argList, optionNode, shortNameIndex);
                return;
            }
            
            var optionName = shortNameIndex == -1 ? $"--{option.LongName}" : $"-{option.ShortName}";
            
            // 非Switch Short Option必须在同组的所有Short Name最后面
            if (shortNameIndex != -1 && shortNameIndex != optionNode.Value.Length - 1)
            {
                throw new InvalidArgsException($"Option '{optionName}' must be after all short names.");
            }
            
            if (option.Type == OptionType.Scalar)
            {
                if (!_IsValidOptionValueNode(optionNode.Next))
                {
                    throw new InvalidArgsException($"Lack value of option '{optionName}'.");
                }

                matchedArgs.Add(optionNode.Next!.Value);
                argList.Remove(optionNode.Next);

                _ConsumeOptionArg(argList, optionNode, shortNameIndex);
            }
            else if (option.Type == OptionType.Sequence)
            {
                _ConsumeOptionArg(argList, optionNode, shortNameIndex);
                
                var argNode = optionNode.Next;
                while (_IsValidOptionValueNode(argNode))
                {
                    matchedArgs.Add(argNode!.Value);

                    var tmpNode = argNode.Next;
                    argList.Remove(argNode);
                    argNode = tmpNode;
                }
            }
        }

        private void _ConsumeOptionArg(LinkedList<string> argList, LinkedListNode<string> optionNode, int shortNameIndex)
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
        }

        private bool _IsValidOptionValueNode(LinkedListNode<string>? node)
        {
            // "--"是option结束符
            return node != null && node.Value != "--";
        }
    }
}