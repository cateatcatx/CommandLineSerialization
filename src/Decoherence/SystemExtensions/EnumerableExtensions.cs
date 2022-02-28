using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace Decoherence.SystemExtensions
{
#if HIDE_DECOHERENCE
    internal static class EnumerableExtensions
#else
    public static class EnumerableExtensions
#endif
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null && action != null)
            {
                foreach (var item in enumerable)
                {
                    action(item);
                }
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var i = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    return i;
                }
                ++i;
            }

            return -1;
        }
        
        [return: MaybeNull]
        public static T Find<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default;
        }
    }
}
