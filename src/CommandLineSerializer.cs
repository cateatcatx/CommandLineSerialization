using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Decoherence.SystemExtensions;
// ReSharper disable UseDeconstruction

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineSerializer
    {
        private readonly IValueSerializer mValueSerializer;

        public CommandLineSerializer(IValueSerializer? valueSerializer = null)
        {
            mValueSerializer = valueSerializer ?? new BuiltinValueSerializer();
        }
        
        public object? DeserializeObject(
            Type objType,
            string commandLine,
            out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(ImplUtil.SplitCommandLine(commandLine));
            return DeserializeObject(objType, remainArgs);
        }

        public object? DeserializeObject(
            Type objType,
            IEnumerable<string> args,
            out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(args);
            return DeserializeObject(objType, remainArgs);
        }
        
        public T DeserializeObject<T>(LinkedList<string> argList)
        {
            var obj = DeserializeObject(typeof(T), argList);
            return (T)obj!;
        }

        public object? DeserializeObject(
            Type objType, 
            LinkedList<string> argList)
        {
            var objectSpecs = mValueSerializer.GetObjectSpecs(objType);
            if (objectSpecs == null)
            {
                throw new InvalidOperationException($"Type {objType} can not deserialize to object.");
            }
            
            objectSpecs.BeginDeserializeObject(this, argList);
            Deserialize(argList, objectSpecs,  (spec, value) => objectSpecs.SpecDeserialized(spec, value));
            return objectSpecs.EndDeserializeObject(this, argList);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <param name="specs">命令行参数说明</param>
        /// <param name="onDeserialized">当反序列化成功时调用</param>
        /// <returns>反序列化后剩余的命令行参数</returns>
        public LinkedList<string> Deserialize(IEnumerable<string> args,
            ISpecs specs,
            OnDeserialized? onDeserialized)
        {
            LinkedList<string> argList = new(args);
            Deserialize(argList, specs, onDeserialized);
            return argList;
        }

        /// <summary>
        /// <inheritdoc cref="Deserialize(System.Collections.Generic.IEnumerable{string},Decoherence.CommandLineSerialization.ISpecs,Decoherence.CommandLineSerialization.OnDeserialized?)"/>
        /// </summary>
        /// <param name="argList">命令行参数，本集合会被修改，调用完毕后集合内是剩余的命令行参数</param>
        /// <param name="specs">命令行参数说明</param>
        /// <param name="onDeserialized">当反序列化成功时调用</param>
        public void Deserialize(LinkedList<string> argList,
            ISpecs specs,
            OnDeserialized? onDeserialized)
        {
            var nodeAfterDemarcate = _DeserializeOptions(argList.First, specs, onDeserialized);
            _DeserializeArguments(argList, nodeAfterDemarcate, specs, onDeserialized);
        }

        public LinkedList<string> SerializeObject(object obj)
        {
            var objType = obj.GetType();
            var objectSpecs = mValueSerializer.GetObjectSpecs(objType);
            if (objectSpecs == null)
            {
                throw new InvalidOperationException($"Type {objType} can not serialize to command line.");
            }
            
            objectSpecs.BeginSerializeObject(this, obj);
            var argList = Serialize(objectSpecs, spec => objectSpecs.SpecSerialized(spec));
            objectSpecs.EndSerializeObject(this, obj);

            return argList;
        }
        
        public LinkedList<string> Serialize(ISpecs specs, OnSerialized? onSerialized)
        {
            LinkedList<string> argList = new();
            _SerializeOptions(specs.Options, argList, onSerialized);
            _SerializeArguments(specs.Arguments, argList, onSerialized);
            return argList;
        }

        #region 序列化相关

        private void _SerializeOptions(IReadOnlyList<IOption> options, LinkedList<string> argList, OnSerialized? onSerialized)
        {
            foreach (var option in options)
            {
                var obj = onSerialized?.Invoke(option);
                var optionPrefix = option.LongName != null ? $"--{option.LongName}" : $"-{option.ShortName}"; // LongName优先
                
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
                    
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }
        }

        private void _SerializeArguments(IEnumerable<IArgument> arguments, LinkedList<string> argList, OnSerialized? onSerialized)
        {
            bool addDemarcate = false;
            
            foreach (var argument in arguments)
            {
                var obj = onSerialized?.Invoke(argument);
                
                if (argument.ValueType == ValueType.Single)
                {
                    if (!addDemarcate)
                    {
                        argList.AddLast("--");
                        addDemarcate = true;
                    }
                    
                    argList.AddLast(mValueSerializer.SerializeSingleValue(this, argument.ObjType, obj));
                    
                    continue;
                }

                if (argument.ValueType == ValueType.Sequence)
                {
                    foreach (string value in mValueSerializer.SerializeMultiValue(this, argument.ObjType, obj))
                    {
                        if (!addDemarcate)
                        {
                            argList.AddLast("--");
                            addDemarcate = true;
                        }
                        
                        argList.AddLast(value);
                    }
                    
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }
        }

        #endregion

        #region 反序列化相关

        private LinkedListNode<string>? _DeserializeOptions(LinkedListNode<string>? node, ISpecs specs, OnDeserialized? onDeserialized)
        {
            Dictionary<IOption, List<string>> parsingMultiValueOptions = new();
            StringBuilder shortOptionHolder = new();
            HashSet<IOption> parsedOptions = new();
            LinkedListNode<string>? nodeAfterDemarcate = null;
            IOption? parsingOption = null;
            
            // 按顺序处理每个参数，解析option
            while (node != null)
            {
                var arg = node.Value;

                var m = _MatchArg(arg);
                if (_IsDemarcated(m))
                {// 处理"--"
                    if (parsingOption != null)
                    {
                        _ParseParsingOption(null, onDeserialized, parsingOption, parsingMultiValueOptions);
                    }

                    nodeAfterDemarcate = node.Next;
                    break;
                }

                if (parsingOption != null)
                {// 这里优先解析parsingOption的Value，就算当前arg由-开头（比如-a、--aaa）也当做value处理
                    parsingOption = _ParseParsingOption(arg, onDeserialized, parsingOption, parsingMultiValueOptions);
                    node = _ConsumeNode(node);
                    continue;
                }

                if (_IsLongOption(m))
                {
                    var optionName = m.Groups[3].Value;
                    var appendedValue = m.Groups[4].Value == "=" ? m.Groups[5].Value : null; // 有"="时可能空值可能有值，没有则为null
                    
                    if (_TryGetOption(optionName, specs, parsedOptions, out var option))
                    {
                        _DeserializeOption(onDeserialized, option, appendedValue, parsingMultiValueOptions, ref parsingOption);
                        node = _ConsumeNode(node);
                    }
                    else
                    {
                        node = node.Next;
                    }
                    
                    continue;
                }

                if (_IsShortOption(m))
                {
                    shortOptionHolder.Clear();
                    shortOptionHolder.Append(arg);
                    
                    for (int i = 1; i < shortOptionHolder.Length; ++i)
                    {
                        if (_TryGetOption(shortOptionHolder[i].ToString(), specs, parsedOptions, out var option))
                        {
                            int removeCount = shortOptionHolder.Length - i;
                            string? appendedValue = null;
                            
                            if (option.ValueType == ValueType.Non)
                            {
                                removeCount = 1;
                            }
                            else if (i < shortOptionHolder.Length - 1)
                            {// 只要不是NonValue，在后面还有字符的情况下，都要把字符吃光变成值
                                appendedValue = shortOptionHolder.ToString(i + 1, shortOptionHolder.Length - (i + 1));
                            }

                            _DeserializeOption(onDeserialized, option, appendedValue, parsingMultiValueOptions, ref parsingOption);
                            shortOptionHolder.Remove(i, removeCount);
                        }
                    }

                    if (shortOptionHolder.Length <= 1)
                    {
                        node = _ConsumeNode(node);
                    }
                    else
                    {
                        node.Value = shortOptionHolder.ToString();
                        node = node.Next;
                    }
                    
                    continue;
                }
                
                node = node.Next;
            }
            
            // 处理多值option
            foreach (var kv in parsingMultiValueOptions)
            {
                var option = kv.Key;
                var values = kv.Value;
                
                onDeserialized?.Invoke(option, _DeserializeMultiValue(option, values));
                parsedOptions.Add(option);
            }
            
            // 反序列化未匹配的Non类型值
            foreach (var option  in specs.Options)
            {
                if (option.ValueType != ValueType.Non || parsedOptions.Contains(option))
                {
                    continue;
                }
                
                onDeserialized?.Invoke(option, _DeserializeNonValue(option, false));
            }

            return nodeAfterDemarcate;
        }

        private void _DeserializeArguments(LinkedList<string> argList, LinkedListNode<string>? nodeAfterDemarcate, ISpecs specs, OnDeserialized? onDeserialized)
        {
            if (specs.Arguments.Count <= 0)
                return;

            var arguments = specs.Arguments;
            List<string> parsingArgumentValues = new();
            IArgument? parsingArgument = null;
            var i = 0;
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
                    parsingArgumentValues.Add(arg);
                    node = _ConsumeNode(node);
                    continue;
                }

                if (i >= arguments.Count)
                {
                    if (parsingArgument == null)
                    {// 无事可做直接退出循序
                        break;
                    }

                    continue;
                }

                var argument = arguments[i++];
                if (argument.ValueType == ValueType.Single)
                {
                    onDeserialized?.Invoke(argument, _DeserializeSingleValue(argument, arg));
                }
                else if (argument.ValueType == ValueType.Sequence)
                {
                    parsingArgumentValues.Add(arg);
                    parsingArgument = argument;
                }

                node = _ConsumeNode(node);
            }

            // 处理多值argument
            if (parsingArgument != null)
            {
                onDeserialized?.Invoke(parsingArgument, _DeserializeMultiValue(parsingArgument, parsingArgumentValues));
            }
        }
        
        private IOption? _ParseParsingOption(string? arg, OnDeserialized? onDeserialized, IOption parsingOption, IReadOnlyDictionary<IOption, List<string>> parsingMultiValueOptions)
        {
            if (parsingOption.ValueType == ValueType.Single)
            {
                onDeserialized?.Invoke(parsingOption, _DeserializeSingleValue(parsingOption, arg));
                return null;
            }
            
            if (parsingOption.ValueType is ValueType.Multi or ValueType.Sequence)
            {
                if (arg != null)
                    parsingMultiValueOptions[parsingOption].Add(arg);

                if (parsingOption.ValueType is ValueType.Multi)
                {
                    return null;
                }
            }

            return parsingOption;
        }
        
        private bool _TryGetOption(string optionName, ISpecs specs, ISet<IOption> parsedOptions, [NotNullWhen(true)] out IOption? option)
        {
            if (specs.TryGetOption(optionName, out option))
            {
                if (!parsedOptions.Contains(option))
                {
                    if (option.ValueType != ValueType.Multi)
                    {// -a1 -a2 -a3 可以匹配多次
                        parsedOptions.Add(option);
                    }

                    return true;
                }
            }

            return false;
        }

        private void _DeserializeOption(OnDeserialized? onDeserialized, IOption option, string? value, IDictionary<IOption, List<string>> parsingMultiValueOptions, ref IOption? parsingOption)
        {
            if (option.ValueType == ValueType.Non)
            {
                onDeserialized?.Invoke(option, _DeserializeNonValue(option, true));
            }
            else if (option.ValueType == ValueType.Single)
            {
                if (value != null)
                {
                    onDeserialized?.Invoke(option,_DeserializeSingleValue(option, value));
                }
                else
                {
                    parsingOption = option;
                }
            }
            else if (option.ValueType is ValueType.Multi or ValueType.Sequence)
            {
                var values = parsingMultiValueOptions.AddOrCreateValue(option, () => new List<string>());
                if (value != null)
                {
                    values.Add(value);
                }

                if (value == null || option.ValueType == ValueType.Sequence)
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
        
        private object? _DeserializeNonValue(ISpec spec, bool matched)
        {
            return spec.CanHandleType(spec.ObjType) ? spec.DeserializeNonValue(this, spec.ObjType, matched) : mValueSerializer.DeserializeNonValue(this, spec.ObjType, matched);
        }

        private object? _DeserializeSingleValue(ISpec spec, string? value)
        {
            return spec.CanHandleType(spec.ObjType) ? spec.DeserializeSingleValue(this, spec.ObjType, value) : mValueSerializer.DeserializeSingleValue(this, spec.ObjType, value);
        }

        private object? _DeserializeMultiValue(ISpec spec, List<string> values)
        {
            return spec.CanHandleType(spec.ObjType) ? spec.DeserializeMultiValue(this, spec.ObjType, values) : mValueSerializer.DeserializeMultiValue(this, spec.ObjType, values);
        }
        
        #endregion
    }
}