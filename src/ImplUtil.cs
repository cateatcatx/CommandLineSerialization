using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Decoherence.CommandLineSerialization.Attributes;

namespace Decoherence.CommandLineSerialization
{
    public static class ImplUtil
    {
        public static ISpec GenerateSpecByAttribute(
            SpecAttribute attr, 
            Type objType, 
            string defaultOptionName,
            ValueType defaultValueType,
            IValueSerializer? defaultValueSerializer)
        {
            if (attr is ArgumentAttribute argumentAttr)
            {
                return new Argument(argumentAttr.ValueType != ValueType.Default ? argumentAttr.ValueType : defaultValueType, 
                    objType, 
                    argumentAttr.Priority, argumentAttr.Serializer ?? defaultValueSerializer);
            }

            var optionAttr = (OptionAttribute)attr;
            return new Option(
                optionAttr.Name ?? defaultOptionName, 
                optionAttr.ValueType != ValueType.Default ? optionAttr.ValueType : defaultValueType, 
                objType, 
                optionAttr.Serializer ?? defaultValueSerializer);
        }

        public static LinkedList<string> SplitCommandLine(string commandLine)
        {
            LinkedList<string> argList = new();
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return argList;
            }

            string state;
            bool unescapeAny;
            HashSet<char> unescapeChs = new();
            Dictionary<char, string> ch2SpecialCh = new();

            void enterNormalState()
            {
                state = "normal";
                
                ch2SpecialCh.Clear();
                ch2SpecialCh['\"'] = "\"";
                ch2SpecialCh['\''] = "'";
                ch2SpecialCh[' '] = " ";
                ch2SpecialCh['\t'] = "\t";
                
                unescapeAny = true;
            }
            
            void enterDoubleQuote()
            {
                state = "double_quote";
                
                ch2SpecialCh.Clear();
                ch2SpecialCh['\"'] = "\"";
                
                unescapeAny = false;
                unescapeChs.Clear();
                unescapeChs.Add('\\');
                unescapeChs.Add('"');
            }
            
            void enterSingleQuote()
            {
                state = "single_quote";
                
                ch2SpecialCh.Clear();
                ch2SpecialCh['\''] = "'";
                
                unescapeAny = false;
                unescapeChs.Clear();
                unescapeChs.Add('\\');
            }

            enterNormalState();
            StringBuilder sb = new();
            bool lastSplitCh = true;

            for (var i = 0; i < commandLine.Length; ++i)
            {
                string? specialCh = null;
                var ch = commandLine[i];
                if (ch == '\\' && (unescapeAny || unescapeChs.Count > 0))
                {
                    if (i == commandLine.Length - 1)
                    {
                        specialCh = "null";
                    }
                    else if (unescapeAny || unescapeChs.Contains(commandLine[i + 1]))
                    {
                        ch = commandLine[++i];
                    }
                }
                else
                {
                    ch2SpecialCh.TryGetValue(ch, out specialCh);
                }

                if (specialCh == "null")
                    continue;
                
                if (specialCh == "\"")
                {
                    if (state == "double_quote")
                    {
                        enterNormalState();
                    }
                    else
                    {
                        enterDoubleQuote();
                    }
                }
                else if (specialCh == "'")
                {
                    if (state == "single_quote")
                    {
                        enterNormalState();
                    }
                    else
                    {
                        enterSingleQuote();
                    }
                }
                else if (!lastSplitCh && (specialCh is " " or "\t"))
                {
                    argList.AddLast(sb.ToString());
                    sb.Clear();
                }
                else if (specialCh == null)
                {
                    sb.Append(ch);
                }

                lastSplitCh = specialCh is " " or "\t";
            }

            if (state is "double_quote" or "single_quote")
            {
                throw new InvalidOperationException($"Split '{commandLine}' error: lack of '\"'.");
            }

            if (!lastSplitCh)
            {
                argList.AddLast(sb.ToString());
            }

            return argList;
        }
    }
}