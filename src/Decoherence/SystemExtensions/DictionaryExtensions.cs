using System;
using System.Collections.Generic;

namespace Decoherence.SystemExtensions
{
    #if HIDE_DECOHERENCE
    internal static class DictionaryExtensions
#else
#if HIDE_DECOHERENCE
    internal static class DictionaryExtensions
#else
    public static class DictionaryExtensions
#endif
#endif
    {
        public static TValue AddOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TValue> createFunc)
        {
            if (!self.TryGetValue(key, out var value))
            {
                value = createFunc();
                self.Add(key, value);
            }

            return value;
        }
        
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (!self.ContainsKey(key))
            {
                self.Add(key, value);
                return true;
            }
            
            return false;
        }
    }
}