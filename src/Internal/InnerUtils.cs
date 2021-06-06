using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    internal static class InnerUtils
    {
        public static bool TryFind<T>(this LinkedList<T> list, Func<T, bool> condition, out LinkedListNode<T>? nodeFinded)
        {
            nodeFinded = null;
            
            var node = list.First;
            while (node != null)
            {
                if (condition(node.Value))
                {
                    nodeFinded = node;
                    return true;
                }

                node = node.Next;
            }

            return false;
        }
    }
}