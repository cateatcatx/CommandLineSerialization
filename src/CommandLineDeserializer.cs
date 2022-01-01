using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineDeserializer
    {
        private readonly IValueSerializer mValueSerializer;
        private readonly Dictionary<IOption, List<string>> mParsingMultiValueOptions;
        private readonly StringBuilder mShortOptionHolder;
        private readonly HashSet<IOption> mParsedOptions;
        private readonly List<string> mParsingArgumentValues;

        public CommandLineDeserializer(IValueSerializer? valueSerializer = null)
        {
            mValueSerializer = valueSerializer ?? new BuiltinValueSerializer();
            mParsingMultiValueOptions = new Dictionary<IOption, List<string>>(new OptionEqualityComparer());
            mShortOptionHolder = new StringBuilder();
            mParsedOptions = new HashSet<IOption>(new OptionEqualityComparer());
            mParsingArgumentValues = new List<string>();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="args">分割后的命令行参数</param>
        /// <param name="specs">参数说明</param>
        /// <param name="optionDeserialized">当反序列化成功一个option时调用</param>
        /// <param name="argumentDeserialized">当反序列化成功一个argument时调用</param>
        /// <returns>反序列化后剩余的命令行参数</returns>
        public LinkedList<string> Deserialize(
            IEnumerable<string> args, 
            ISpecs specs,
            Action<IOption, object?>? optionDeserialized,
            Action<IArgument, object?>? argumentDeserialized)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (specs == null)
                throw new ArgumentNullException(nameof(specs));

            LinkedList<string> argList = new(args);
            
            var nodeAfterDemarcate = _ParseOptions(argList.First, specs, optionDeserialized);
            _ParseArguments(argList, nodeAfterDemarcate, specs, argumentDeserialized);
            
            return argList;
        }

        private LinkedListNode<string>? _ParseOptions(LinkedListNode<string>? node, ISpecs specs, Action<IOption, object?>? optionDeserialized)
        {
            mParsingMultiValueOptions.Clear();
            mParsedOptions.Clear();
            LinkedListNode<string>? nodeAfterDemarcate = null;
            IOption? parsingOption = null;
            
            while (node != null)
            {
                var arg = node.Value;

                var m = _MatchArg(arg);
                if (_IsDemarcated(m))
                {// 处理"--"
                    if (parsingOption != null)
                    {
                        _ParseParsingOption(null, optionDeserialized, parsingOption);
                    }

                    nodeAfterDemarcate = node.Next;
                    break;
                }

                if (parsingOption != null)
                {// 这里优先解析parsingOption的Value，就算当前arg由-开头（比如-a、--aaa）也当做value处理
                    parsingOption = _ParseParsingOption(arg, optionDeserialized, parsingOption);
                    node = _ConsumeNode(node);
                    continue;
                }

                if (_IsLongOption(m))
                {
                    var optionName = m.Groups[3].Value;
                    var appendedValue = m.Groups[4].Value == "=" ? m.Groups[5].Value : null; // 有"="时可能空值可能有值，没有则为null
                    
                    if (_TryGetOption(optionName, specs, out var option))
                    {
                        _DeserializeOption(optionDeserialized, option, appendedValue, ref parsingOption);
                    }

                    node = _ConsumeNode(node);
                    continue;
                }

                if (_IsShortOption(m))
                {
                    mShortOptionHolder.Clear();
                    mShortOptionHolder.Append(arg);
                    
                    for (int i = 1; i < mShortOptionHolder.Length; ++i)
                    {
                        if (_TryGetOption(mShortOptionHolder[i].ToString(), specs, out var option))
                        {
                            int removeCount = mShortOptionHolder.Length - i;
                            string? appendedValue = null;
                            
                            if (option.ValueType == OptionValueType.None)
                            {
                                removeCount = 1;
                            }
                            else if (i < mShortOptionHolder.Length - 1)
                            {// 只要不是NoneValue，在后面还有字符的情况下，都要把字符吃光变成值
                                appendedValue = mShortOptionHolder.ToString(i + 1, mShortOptionHolder.Length - (i + 1));
                            }

                            _DeserializeOption(optionDeserialized, option, appendedValue, ref parsingOption);
                            mShortOptionHolder.Remove(i, removeCount);
                        }
                    }

                    if (mShortOptionHolder.Length <= 1)
                    {
                        node = _ConsumeNode(node);
                    }
                    else
                    {
                        node.Value = mShortOptionHolder.ToString();
                        node = node.Next;
                    }
                    
                    continue;
                }
                
                node = node.Next;
            }
            
            // 处理多值option
            foreach (var option2values in mParsingMultiValueOptions)
            {
                var option = option2values.Key;
                var values = option2values.Value;
                optionDeserialized?.Invoke(option, _DeserializeMultiValue(option, values));
                mParsedOptions.Add(option);
            }
            
            // 处理未匹配的option
            foreach (var option in specs.Options.Values.Where(option => !mParsedOptions.Contains(option)))
            {
                optionDeserialized?.Invoke(option, _DeserializeWhenNoMatch(option));
            }

            return nodeAfterDemarcate;
        }

        private void _ParseArguments(LinkedList<string> argList, LinkedListNode<string>? nodeAfterDemarcate, ISpecs specs, Action<IArgument, object?>? argumentDeserialized)
        {
            if (specs.Arguments.Count <= 0)
                return;

            mParsingArgumentValues.Clear();
            var i = 0;
            IArgument? parsingArgument = null;
            var node = nodeAfterDemarcate ?? argList.First;
            while (node != null)
            {
                var arg = node.Value;

                if (nodeAfterDemarcate == null && !_IsNonOption(_MatchArg(arg)))
                {// 跳过"-"开头

                    node = node.Next;
                    continue;
                }

                if (parsingArgument != null)
                {
                    mParsingArgumentValues.Add(arg);
                    node = _ConsumeNode(node);
                    continue;
                }

                var argument = specs.Arguments[i++];
                if (argument.ValueType == ArgumentValueType.Single)
                {
                    argumentDeserialized?.Invoke(argument, _DeserializeSingleValue(argument, arg));
                }
                else if (argument.ValueType == ArgumentValueType.Sequence)
                {
                    mParsingArgumentValues.Add(arg);
                    parsingArgument = null;
                }

                node = _ConsumeNode(node);
            }

            // 处理多值argument
            if (parsingArgument != null)
            {
                argumentDeserialized?.Invoke(parsingArgument, _DeserializeMultiValue(parsingArgument, mParsingArgumentValues));
            }
            
            // 处理剩余arguments
            for (; i < specs.Arguments.Count; ++i)
            {
                var argument = specs.Arguments[i];
                argumentDeserialized?.Invoke(argument, _DeserializeWhenNoMatch(argument));
            }
        }
        
        private IOption? _ParseParsingOption(string? arg, Action<IOption, object?>? optionDeserialized, IOption parsingOption)
        {
            if (parsingOption.ValueType == OptionValueType.Single)
            {
                optionDeserialized?.Invoke(parsingOption, _DeserializeSingleValue(parsingOption, arg));
                return null;
            }
            
            if (parsingOption.ValueType is OptionValueType.Multi or OptionValueType.Sequence)
            {
                if (arg != null)
                    mParsingMultiValueOptions[parsingOption].Add(arg);

                if (parsingOption.ValueType is OptionValueType.Multi)
                {
                    return null;
                }
            }

            return parsingOption;
        }
        
        private bool _TryGetOption(string optionName, ISpecs specs, [NotNullWhen(true)] out IOption? option)
        {
            if (specs.Options.TryGetValue(optionName, out option))
            {
                if (!mParsedOptions.Contains(option))
                {
                    if (option.ValueType != OptionValueType.Multi)
                    {// -a1 -a2 -a3 可以匹配多次
                        mParsedOptions.Add(option);
                    }

                    return true;
                }
            }

            return false;
        }

        private void _DeserializeOption(Action<IOption, object?>? optionDeserialized, IOption option, string? value, ref IOption? parsingOption)
        {
            if (option.ValueType == OptionValueType.None)
            {
                optionDeserialized?.Invoke(option, _DeserializeNoneValue(option));
            }
            else if (option.ValueType == OptionValueType.Single)
            {
                if (value != null)
                {
                    optionDeserialized?.Invoke(option, _DeserializeSingleValue(option, value));
                }
                else
                {
                    parsingOption = option;
                }
            }
            else if (option.ValueType is OptionValueType.Multi or OptionValueType.Sequence)
            {
                var values = mParsingMultiValueOptions.AddOrCreateValue(option, () => new List<string>());
                if (value != null)
                {
                    values.Add(value);
                }

                if (value == null || option.ValueType == OptionValueType.Sequence)
                {
                    parsingOption = option;
                }
            }
        }

        private LinkedListNode<string>? _ConsumeNode(LinkedListNode<string> node)
        {
            var nextNode = node.Next;
            node.List.Remove(node);
            return nextNode;
        }

        private Match _MatchArg(string arg)
        {
            return Regex.Match(arg, "^(-)?(-)?([^=\n\r]*)(=)?(.*)$");
        }

        /// <summary>
        /// 是否为--
        /// </summary>
        private bool _IsDemarcated(Match match)
        {
            return match.Length == 2 && match.Success && match.Groups[1].Value == "-" && match.Groups[2].Value == "-";
        }

        /// <summary>
        /// 如 --aaa --aaa= --aaa=123
        /// </summary>
        private bool _IsLongOption(Match match)
        {
            return match.Length > 2 && match.Success && match.Groups[1].Value == "-" && match.Groups[2].Value == "-";
        }

        /// <summary>
        /// 如 -a -abc1
        /// </summary>
        private bool _IsShortOption(Match match)
        {
            return match.Success && match.Groups[1].Value == "-" && match.Groups[2].Value != "-";
        }

        /// <summary>
        /// 如 aaa 111
        /// </summary>
        private bool _IsNonOption(Match match)
        {
            return !match.Success || match.Groups[1].Value != "-";
        }
        
        private object? _DeserializeNoneValue(ISpec spec)
        {
            return spec.IsControlSerialization ? spec.DeserializeNoneValue(spec.ObjType) : mValueSerializer.DeserializeNoneValue(spec.ObjType);
        }

        private object? _DeserializeSingleValue(ISpec spec, string? value)
        {
            return spec.IsControlSerialization ? spec.DeserializeSingleValue(spec.ObjType, value) : mValueSerializer.DeserializeSingleValue(spec.ObjType, value);
        }

        private object? _DeserializeMultiValue(ISpec spec, List<string> values)
        {
            return spec.IsControlSerialization ? spec.DeserializeMultiValue(spec.ObjType, values) : mValueSerializer.DeserializeMultiValue(spec.ObjType, values);
        }
        
        private object? _DeserializeWhenNoMatch(ISpec spec)
        {
            return spec.IsControlSerialization ? spec.DeserializeWhenNoMatch(spec.ObjType) : mValueSerializer.DeserializeWhenNoMatch(spec.ObjType);
        }
    }
}