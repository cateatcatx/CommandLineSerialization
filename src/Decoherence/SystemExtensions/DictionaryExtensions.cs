using System;
using System.Collections.Generic;

namespace Decoherence.SystemExtensions
{
    public static class DictionaryExtensions
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