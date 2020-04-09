using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HashSet<string> ToCaseInsensitiveHashSet(this IEnumerable<string> source)
        {
            return new HashSet<string>(source, StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> source, int pageSize)
        {
            var l = source.ToList();

            for (var i = 0; i < l.Count; i += pageSize)
            {
                yield return l.Skip(i).Take(pageSize);
            }
        }

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> list)
        {
            return list?.ToArray() ?? Array.Empty<T>();
        }
    }
}