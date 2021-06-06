using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Decoherence.CommandLineParsing
{
    public class CommandLineSerializer
    {
        private readonly SerializationStrategy mSerializationStrategy;

        public CommandLineSerializer(SerializationStrategy? serializationStrategy = null)
        {
            mSerializationStrategy = serializationStrategy ?? new SerializationStrategy();
        }

        public Values DeserializeValues(IEnumerable<string> args, Specs specs)
        {
            return DeserializeValues(args, specs, out _);
        }
        
        public Values DeserializeValues(IEnumerable<string> args, Specs specs, out LinkedList<string> remainArgs)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (specs == null)
                throw new ArgumentNullException(nameof(specs));

            var values = new Values();
            remainArgs = new LinkedList<string>(args);
            List<string> helperArgList = new();

            // --- 先解析Option ---
            List<Option> options = new(specs.Options);
            
            // 按switch、scalar、sequence排序，这样可以保证在处理ShortName时先消耗掉Switch，这样合法的Scalar和Sequence就在最后了
            options.Sort((a, b) => (int)a.Type - (int)b.Type);
            
            foreach (var option in options)
            {
                IEnumerable<string>? matchedArgs = null;
                object? value = null;

                if (!_TryMatchOption(option, remainArgs, out var optionMatch))
                {
                    value = _HandleNoArg(option);
                }
                else
                {
                    if (option.Type == OptionType.Switch)
                    {
                        optionMatch!.ConsumeArgs();
                    }
                    else if (option.Type == OptionType.Scalar)
                    {
                        helperArgList.Clear();
                        matchedArgs = helperArgList;
                        optionMatch!.FillMatchedArgs(helperArgList);
                        optionMatch!.ConsumeArgs();
                    }
                    else if (option.Type == OptionType.Sequence)
                    {
                        helperArgList.Clear();
                        matchedArgs = helperArgList;

                        while (optionMatch != null)
                        {
                            optionMatch!.FillMatchedArgs(helperArgList);
                            var nextNode = optionMatch!.ConsumeArgs();
                        }
                    }

                    value = DeserializeValue(option.ValueType, matchedArgs);
                }

                values.AddValue(option, value);
                
                
                
                
                
                
                var matched = _MatchOptionAndConsumeArgs(option, remainArgs, helperArgList);
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
                    value = DeserializeValue(option.ValueType, helperArgList);
                }
                
                
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

            return values;
        }

        public string SerializeValue(Type valueType, object? value)
        {
            throw new NotImplementedException();
        }

        public object? DeserializeValue(Type valueType, IEnumerable<string>? args)
        {
            var func = mSerializationStrategy.GetDeserializeFunc(valueType);
            return func(this, valueType, args);
        }

        private bool _MatchOptionAndConsumeArgs(Option option, LinkedList<string> argList, List<string> matchedArgs)
        {
            // LongName比ShortName更具体，所以先匹配LongName
            // 格式：
            //  --arg1 100
            //  --arg1=100
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
                    if (_TryGetShortNameIndex(tmpNode, option, out shortNameIndex))
                    {
                        node = tmpNode;
                        break;
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
        
        private bool _TryGetShortNameIndex(LinkedListNode<string> node, Option option, out int shortNameIndex)
        {
            shortNameIndex = -1;
            
            if (option.ShortName != null && node.Value.StartsWith("-"))
            {
                shortNameIndex = node.Value.IndexOf(option.ShortName.Value);
                return shortNameIndex > 0;
            }

            return false;
        }

        /// <param name="option"></param>
        /// <param name="argList"></param>
        /// <param name="matchedArgs"></param>
        /// <param name="optionNode"></param>
        /// <param name="shortNameIndex">-1代表长Option</param>
        /// <exception cref="InvalidArgsException"></exception>
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

            if (option.Type == OptionType.Scalar)
            {
                var optionValue = _GetAndConsumeFirstOptionValue(argList, optionNode, shortNameIndex, out _);
                if (optionValue == null)
                {
                    var optionName = shortNameIndex == -1 ? $"--{option.LongName}" : $"-{option.ShortName}";
                    throw new InvalidArgsException($"Lack scalar option value of '{optionName}'.");
                }

                matchedArgs.Add(optionValue);
            }
            else if (option.Type == OptionType.Sequence)
            {
                var first = true;
                var nextNode = optionNode;
                while (nextNode != null)
                {
                    string? optionValue = null;
                    if (first)
                    {
                        first = false;
                        optionValue = _GetAndConsumeFirstOptionValue(argList, optionNode, shortNameIndex, out nextNode);
                    }
                    else
                    {
                        if (option.LongName != null && nextNode.Value.StartsWith($"--{option.LongName}"))
                        {
                            optionValue = _GetAndConsumeFirstOptionValue(argList, nextNode, -1, out nextNode);
                        }
                        else if (_TryGetShortNameIndex(nextNode, option, out var tmpShortNameIndex))
                        {
                            optionValue = _GetAndConsumeFirstOptionValue(argList, nextNode, tmpShortNameIndex, out nextNode);
                        }
                        else
                        {
                            optionValue = nextNode.Value;
                            nextNode = nextNode.Next;
                        }
                    }
                    
                    if (optionValue != null)
                    {
                        matchedArgs.Add(optionValue);
                    }
                }
            }
        }

        private string? _GetAndConsumeFirstOptionValue(LinkedList<string> argList, LinkedListNode<string> optionNode, int shortNameIndex, out LinkedListNode<string>? nextNode)
        {
            string? optionValue = null;

            if (shortNameIndex == -1)
            {// long option
                
                var equalSignIndex = optionNode.Value.IndexOf('=');
                if (equalSignIndex > 0)
                {// 形如--arg1=100
                    nextNode = optionNode.Next;
                    
                    if (equalSignIndex == optionNode.Value.Length - 1)
                    {
                        return null;
                    }

                    optionValue = optionNode.Value.Substring(equalSignIndex + 1);
                    argList.Remove(optionNode);
                }
                else
                {// 形如--arg1 100
                    return _GetAndConsumeSplitOptionValue(argList, optionNode, out nextNode);
                }
            }
            else
            {// short option
                
                if (shortNameIndex < optionNode.Value.Length - 1)
                {// 形如-ba100
                    nextNode = optionNode.Next;
                    
                    optionValue = optionNode.Value.Substring(shortNameIndex + 1);
                    optionNode.Value = optionNode.Value.Substring(0, shortNameIndex);
                    if (string.IsNullOrWhiteSpace(optionNode.Value))
                    {// 如果全部吃光，则删除arg
                        argList.Remove(optionNode);
                    }
                }
                else
                {// 形如 -a 100
                    return _GetAndConsumeSplitOptionValue(argList, optionNode, out nextNode);
                }
            }

            return optionValue;
        }

        private static string? _GetAndConsumeSplitOptionValue(LinkedList<string> argList, LinkedListNode<string> optionNode, out LinkedListNode<string>? nextNode)
        {
            if (optionNode.Next == null)
            {
                nextNode = null;
                return null;
            }
            
            if (optionNode.Next.Value == "--")
            {
                nextNode = null;
                return null;
            }
            
            nextNode = optionNode.Next.Next;

            var optionValue = optionNode.Next.Value;
            argList.Remove(optionNode.Next);
            argList.Remove(optionNode);
            return optionValue;
        }

        private bool _IsValidOptionValueNode(LinkedListNode<string> node)
        {
            // "--"是option结束符
            return node.Value != "--";
        }

        private bool _TryMatchOption(Option option, LinkedList<string> args, out OptionMatch? optionMatch)
        {
            // LongName比ShortName更具体，所以先匹配LongName
            // 格式：
            //  --arg1 100
            //  --arg1=100
            if (option.LongName == null || !args.TryFind(arg => $"--{option.LongName}" == arg, out optionNode))
            {
                if (option.ShortName != null)
                {
                    remainArgs.TryFind(arg => _MatchShortName(option.ShortName.Value, arg), out optionNode);
                }
            }
        }

        private bool _MatchLongName(string longName, string arg)
        {
            return $"--{longName}" == arg;
        }

        private bool _MatchShortName(char shortName, string arg)
        {
            throw new NotImplementedException();
        }

        private object? _HandleNoArg(Spec spec)
        {
            throw new NotImplementedException();
        }
    }
}