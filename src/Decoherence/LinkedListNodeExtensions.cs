using System.Collections.Generic;

namespace Decoherence.SystemExtensions
{
    public static class LinkedListNodeExtensions
    {
        public static LinkedListNode<T>? FindNext<T>(this LinkedListNode<T> node, T value)
        {
            var ret = node.Next;
            while (ret is not null && !Equals(ret.Value, value))
            {
                ret = ret.Next;
            }

            return ret;
        }
    }
}