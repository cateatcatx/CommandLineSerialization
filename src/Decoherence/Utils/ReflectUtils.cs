using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence
{
    public static class ReflectUtils
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
    }
}