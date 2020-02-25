using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// As case insensitive read only dictionary.
        /// ordinal ignore case, because it's faster than it's invariant culture cousin
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>a case insensitive readonly dictionary</returns>
        public static IReadOnlyDictionary<string, TValue> ToCaseInsensitiveDictionary<TValue>(this IReadOnlyDictionary<string, TValue> source)
        {
            if (source == null)
            {
                return new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
            }

            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
