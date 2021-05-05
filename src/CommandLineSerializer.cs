using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Decoherence.CommandLineParsing.Exceptions;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineParsing
{
    public class CommandLineSerializer
    {
        public void DeserializeValues(string[] args, Specs specs, out Values values, out string[] remainArgs)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (specs == null)
                throw new ArgumentNullException(nameof(specs));

            values = new Values();
            LinkedList<string> argList = new(args);
            List<string> matchedArgs = new();

            // 先解析Option
            foreach (var option in specs.Options)
            {
                object? value;
                
                matchedArgs.Clear();
                var matched = _TryMatchOption(argList, matchedArgs);
                if (!matched)
                {
                    if (option.Required)
                    {
                        throw new LackRequiredException($"Lack of required option '{option.Name}'.");
                    }

                    value = option.DefaultValueCreator!();
                }
                else
                {
                    if (option.Type == OptionType.Switch)
                    {
                        value = matched;
                    }
                    else if (option.Type == OptionType.Scalar)
                    {
                        value = _ParseScalarValue(option, matchedArgs[0]);
                    }
                    else if (option.Type == OptionType.Sequence)
                    {
                        value = _ParseSequenceValue(option, matchedArgs);
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
                var first = argList.First;

                if (first == null)
                {
                    if (argument.Required)
                    {
                        throw new LackRequiredException($"Lack of required argument '{argument.Name}'.");
                    }

                    value = argument.DefaultValueCreator!();
                }
                else
                {
                    if (argument.Type == ArgumentType.Scalar)
                    {
                        value = _ParseScalarValue(argument, first.Value);
                        argList.RemoveFirst();
                    }
                    else if (argument.Type == ArgumentType.Sequence)
                    {
                        value = _ParseSequenceValue(argument, argList);
                        while (argList.First != null)
                        {
                            argList.RemoveFirst();
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

        private bool _TryMatchOption(Option option, LinkedList<string> argList, List<string> matchedArgs)
        {
            if (option.LongName != null)
            {
                var node = argList.Find($"--{option.LongName}");
                if (node != null)
                {
                    if (option.Type == OptionType.Switch)
                    {
                        argList.Remove(node);
                        return true;
                    }
                    
                    if (option.Type == OptionType.Scalar)
                    {
                        if (node.Next == null)
                        {
                            throw new LackOptionValueException($"Lack value of scalar option '{option.Name}'.");
                        }
                        
                        matchedArgs.Add(node.Value);
                        argList.Remove(node.Next);
                        argList.Remove(node);
                        return true;
                    }

                    if (option.Type == OptionType.Sequence)
                    {
                        node.FindNext($"--{option.LongName}")
                    }
                }
            }
        }

        private object? _ParseScalarValue(Spec spec, string arg)
        {
            throw new NotImplementedException();
        }
        
        private object? _ParseSequenceValue(Spec spec, IEnumerable<string> args)
        {
            throw new NotImplementedException();
        }
    }
}