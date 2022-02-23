using System;
using System.Collections;
using System.Reflection;

namespace Decoherence
{
#if HIDE_DECOHERENCE
    internal static class ReflectUtil
#else
    public static class ReflectUtil
#endif
    {
        public static Array CreateArray(Type type, int length)
        {
            if (!typeof(Array).IsAssignableFrom(type))
                throw new ArgumentException($"{type} is not {nameof(Array)}");

            return (Array)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public, null, new object[] { length }, null);
        }
        
        public static IList CreateList(Type type, int length)
        {
            if (!typeof(IList).IsAssignableFrom(type))
                throw new ArgumentException($"{type} is not {nameof(IList)}");

            return (IList)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public, null, new object[] { length }, null);
        }
        
        public static Type? GetListItemType(Type listType)
        {
            if (listType.HasElementType)
            {
                return listType.GetElementType();
            }

            var type = _FindGenericListType(listType);
            return type?.GenericTypeArguments[0];
        }

        private static Type? _FindGenericListType(Type? type)
        {
            while (true)
            {
                if (type == null)
                {
                    return null;
                }

                var t = type.GetInterface("IList`1");
                if (t != null)
                {
                    return t;
                }

                type = t?.BaseType;
            }
        }
    }
}