using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Decoherence.CommandLineParsing
{
    internal class OptionMatch
    {
        public string OptionNameWithPrefix
        {
            get;
        }
        
        public string? ValueArg
        {
            get;
        }

        private readonly string mOptionNamePrefix;
        private readonly LinkedListNode<string> mOptionNode;
        private readonly LinkedListNode<string>? mOptionValueNode;
        private readonly char? mShortName;

        public OptionMatch(LinkedListNode<string> optionNode,
            string optionNamePrefix,
            Match match,
            char shortName)
            : this(optionNode, optionNamePrefix, match, shortName, null)
        {
        }
        
        public OptionMatch(LinkedListNode<string> optionNode,
            string optionNamePrefix,
            Match match,
            string longName)
            : this(optionNode, optionNamePrefix, match, null, longName)
        {
        }
        
        private OptionMatch(LinkedListNode<string> optionNode,
            string optionNamePrefix,
            Match match,
            char? shortName,
            string? longName)
        {
            mOptionNode = optionNode;
            mOptionNamePrefix = optionNamePrefix;
            mShortName = shortName;

            var name = mShortName != null ? mShortName.Value.ToString() : longName;
            OptionNameWithPrefix = $"{optionNamePrefix}{name}";

            if (match.Groups.Count >= 1)
            {
                ValueArg = match.Groups[1].Value;
            }
            else
            {
                mOptionValueNode = mOptionNode.Next;
                ValueArg = mOptionValueNode?.Value;
            }
        }
        
        /// <summary>
        /// 消耗arg，并返回下一个有效node
        /// </summary>
        /// <returns>下一个node</returns>
        public LinkedListNode<string>? ConsumeArgs(LinkedList<string> args)
        {
            // 值node总是要消耗的
            if (mOptionValueNode != null)
            {
                args.Remove(mOptionValueNode);
            }

            LinkedListNode<string>? nextNode;
            
            if (mShortName != null)
            {
                var tmp = $"{mShortName.Value}{ValueArg}";
                mOptionNode.Value = mOptionNode.Value.Remove(mOptionNode.Value.IndexOf(tmp, StringComparison.Ordinal), tmp.Length);
                if (mOptionNode.Value == mOptionNamePrefix)
                {
                    nextNode = mOptionNode.Next;
                    args.Remove(mOptionNode);
                    return nextNode;
                }

                return mOptionNode; // 还有没有消耗完的short option
            }
            
            // long option，不管是'--arg=100'还是'--arg 100'，option node都需要删除
            nextNode = mOptionNode.Next;
            args.Remove(mOptionNode);
            return nextNode;
        }
    }
}