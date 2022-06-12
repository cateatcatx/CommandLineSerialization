using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace Decoherence.SystemExtensions;
#if HIDE_DECOHERENCE
internal static class ListExtensions
#else
    public static class ListExtensions
#endif
{
    public static void BubbleSort<T>(this IList<T> list, Func<T, T, int> comparer)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (comparer == null) throw new ArgumentNullException(nameof(comparer));

        for (var i = 0; i < list.Count; ++i)
        {
            for (var j = list.Count - 1; j >= i + 1; --j)
            {
                if (comparer(list[j], list[j - 1]) < 0)
                {
                    var tmp = list[j - 1];
                    list[j - 1] = list[j];
                    list[j] = tmp;
                }
            }
        }
    }
        
    public static void Sort<T>(this List<T> list, int index, Func<T, T, int> comparer)
    {
        list.Sort(index, list.Count - index, new Comparer<T>(comparer));
    }

    public static void Sort<T>(this List<T> list, int index, int count, Func<T, T, int> comparer)
    {
        list.Sort(index, count, new Comparer<T>(comparer));
    }

    public static void SwapItem<T>(this IList<T> list, int targetIndex, Func<T, bool> predicate)
    {
        ThrowUtil.ThrowIfArgumentNull(list);
        ThrowUtil.ThrowIfArgumentNull(predicate);
        if (targetIndex >= list.Count) throw new ArgumentOutOfRangeException(nameof(targetIndex));

        var index = list.FindIndex(predicate);
        if (index < 0 || index == targetIndex)
        {
            return;
        }

        T v = list[targetIndex];
        list[targetIndex] = list[index];
        list[index] = v;
    }

    public static bool Remove<T>(this IList<T> list, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public static void ModifyEach<T>(this IList<T> list, Func<T, T> modification)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            list[i] = modification(list[i]);
        }
    }

#if NET35
        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T result)
#else
    public static bool TryFind<T>(this IReadOnlyList<T> list, Predicate<T> match, out T result)
#endif
    {
        ThrowUtil.ThrowIfArgumentNull(list);
        ThrowUtil.ThrowIfArgumentNull(match);

        result = default;

        foreach (var item in list)
        {
            if (match(item))
            {
                result = item;
                return true;
            }
        }

        return false;
    }

#if !NET35
    [return: MaybeNull]
    public static T Find<T>(this IReadOnlyList<T> list, Predicate<T> match)
    {
        foreach (var item in list)
        {
            if (match(item))
            {
                return item;
            }
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"> 找不到匹配的item </exception>
    public static T FindThrow<T>(this IReadOnlyList<T> list, Predicate<T> match)
    {
        ThrowUtil.ThrowIfArgumentNull(list);
        ThrowUtil.ThrowIfArgumentNull(match);

        foreach (var item in list)
        {
            if (match(item))
            {
                return item;
            }
        }

        throw new InvalidOperationException($"Can not find item.");
    }
#endif

    class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> mComparer;

        public Comparer(Func<T, T, int> comparer)
        {
            mComparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return mComparer(x, y);
        }
    }
}