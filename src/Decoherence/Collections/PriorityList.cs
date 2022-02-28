using System;
using System.Collections;
using System.Collections.Generic;

namespace Decoherence
{
    /// <summary>
    /// 优先级列表，item会按优先级的逆序排列（优先级大的在前面）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityList<T>: IReadOnlyList<T>, IList<T>, IList
    {
        private readonly List<T> mList;
        private readonly Func<T, T, int> mPriorityComparer;

        public PriorityList(Func<T, T, int> priorityComparer)
        {
            mList = new List<T>();
            mPriorityComparer = priorityComparer;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)mList).GetEnumerator();
        }

        public void Add(T item)
        {
            _Add(item);
        }

        int IList.Add(object value)
        {
            if (value is not T item)
                throw new ArgumentException($"Not {typeof(T)}", nameof(value));

            return _Add(item);
        }

        void IList.Clear()
        {
            mList.Clear();
        }

        public bool Contains(object value)
        {
            return ((IList)mList).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)mList).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new InvalidOperationException();
        }

        public void Remove(object value)
        {
            ((IList)mList).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            mList.RemoveAt(index);
        }

        public bool IsFixedSize => ((IList)mList).IsFixedSize;

        bool IList.IsReadOnly => ((IList)mList).IsReadOnly;

        object IList.this[int index]
        {
            get => ((IList)mList)[index];
            set => ((IList)mList)[index] = value;
        }

        void ICollection<T>.Clear()
        {
            mList.Clear();
        }

        public bool Contains(T item)
        {
            return mList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            mList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return mList.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)mList).CopyTo(array, index);
        }

        int ICollection.Count => mList.Count;

        public bool IsSynchronized => ((ICollection)mList).IsSynchronized;

        public object SyncRoot => ((ICollection)mList).SyncRoot;

        int ICollection<T>.Count => mList.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)mList).IsReadOnly;

        public int IndexOf(T item)
        {
            return mList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.RemoveAt(int index)
        {
            mList.RemoveAt(index);
        }
        
#if NETSTANDARD2_1

        public T this[Index index]
        {
            get => mList[index];
            set => mList[index] = value;
        }
        
#endif
        
        public T this[int index]
        {
            get => mList[index];
            set => mList[index] = value;
        }

        public int Count => mList.Count;
        
        private int _Add(T item)
        {
            var index = mList.Count;
            for (; index >= 0; --index)
            {
                if (index == 0 || mPriorityComparer(item, mList[index - 1]) <= 0)
                {
                    break;
                }
            }
            
            mList.Insert(index, item);
            return index;
        }
    }
}