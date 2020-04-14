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

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection != null && action != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
        }

        public static void ForAny<T>(this IEnumerable<T> collection, Func<T, bool> matchCondition, Action<T> action)
        {
            if (collection != null && matchCondition != null && action != null)
            {
                collection.ForEach(x =>
                {
                    if (matchCondition(x))
                    {
                        action(x);
                    }
                });
            }
        }

        public static bool NullSafeAny<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            return list != null ? list.Any(expression) : false;
        }

        public static IEnumerable<T> NullSafeWhere<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            return list != null ? list.Where(expression) : Enumerable.Empty<T>();
        }
    }
}