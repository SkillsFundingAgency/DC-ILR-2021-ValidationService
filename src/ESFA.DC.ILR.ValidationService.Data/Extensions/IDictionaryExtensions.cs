using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IDictionaryExtensions
    {
        public static IReadOnlyDictionary<string, TValue> ToCaseInsensitiveDictionary<TValue>(this IReadOnlyDictionary<string, TValue> source)
        {
            if (source == null)
            {
                return new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
            }

            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null || key == null || !dictionary.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            return value;
        }
    }
}
