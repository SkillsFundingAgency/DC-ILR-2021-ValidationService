using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public static class Collection
    {     
        public static bool SafeAny<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            return list != null ? list.Any(expression) : false;
        }

        public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            return list != null ? list.Where(expression) : Enumerable.Empty<T>();
        }

        public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
        {
            It.IsNull(action).AsGuard<ArgumentNullException>(nameof(action));

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }           
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>(nameof(action));

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }                
        }

        public static void ForAny<T>(this IEnumerable<T> list, Func<T, bool> matchCondition, Action<T> doAction)
        {
            It.IsNull(matchCondition)
                .AsGuard<ArgumentNullException>(nameof(matchCondition));
            It.IsNull(doAction)
                .AsGuard<ArgumentNullException>(nameof(doAction));

            if (list != null)
            {
                list.ForEach(x =>
                {
                    if (matchCondition(x))
                    {
                        doAction(x);
                    }
                });
            }           
        }
    }
}
