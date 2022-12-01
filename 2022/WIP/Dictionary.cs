using System;
using System.Collections.Generic;

namespace aoc
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetDef<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> fallback)
        {
            if (dictionary.TryGetValue(key, out var val))
                return val;
            return fallback(key);
        }

        public static TValue TryGetDef<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defValue)
        {
            return dictionary.TryGetDef(key, key => defValue);
        }

        public static TValue Change<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> change){
            var oldValue = dictionary.TryGetDef(key, default(TValue));
            var newValue = change(oldValue);
            dictionary[key] = newValue;
            return newValue;
        }

        public static TValue TryGetDefAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> fallback)
        {
            if (dictionary.TryGetValue(key, out var val))
                return val;
            val = fallback(key);
            dictionary.Add(key, val);
            return val;
        }
    }
}