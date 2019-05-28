using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    /// <summary>
    /// removes the need to 'to list' prior to 'for eaching'
    /// implements a null patterned to safe list
    /// </summary>
    public static class Collection
    {
        /// <summary>
        /// To safe list, null pattern safety
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <returns>an empty collection of <typeparamref name="T"/></returns>
        public static ICollection<T> Empty<T>()
        {
            return Enumerable.Empty<T>().SafeList();
        }

        /// <summary>
        /// Empties the and read only.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <returns>
        /// an empty readonly safe collection
        /// </returns>
        public static IReadOnlyCollection<T> EmptyAndReadOnly<T>()
        {
            return Enumerable.Empty<T>().SafeReadOnlyList();
        }

        /// <summary>
        /// As a safe list, null pattern safety
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe collection
        /// </returns>
        public static ICollection<T> AsSafeList<T>(this IEnumerable<T> list)
        {
            return list.SafeList();
        }

        /// <summary>
        /// As a safe readonly list.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a readonly safe collection
        /// </returns>
        public static IReadOnlyCollection<T> AsSafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return list.SafeReadOnlyList();
        }

        /// <summary>
        /// As safe distinct key set.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe key set collection
        /// </returns>
        public static IContainThis<T> AsSafeDistinctKeySet<T>(this IEnumerable<T> list)
        {
            return new DistinctKeySet<T>(list.SafeReadOnlyList());
        }

        /// <summary>
        /// As a safe distinct case insensitive key set.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe case insensitive key set collection
        /// </returns>
        public static IContainThis<string> AsSafeDistinctKeySet(this IEnumerable<string> list)
        {
            return new CaseInsensitiveDistinctKeySet(list.SafeReadOnlyList());
        }

        /// <summary>
        /// As (a) safe read only digit list.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>
        /// a readonly safe collection
        /// </returns>
        public static IReadOnlyCollection<int> AsSafeReadOnlyDigitList(this long number)
        {
            return Math.Abs(number)
                .ToString()
                .Select(x => Convert.ToInt32(x.ToString()))
                .SafeReadOnlyList();
        }

        /// <summary>
        /// As (a) safe read only digit list.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>
        /// a readonly safe collection
        /// </returns>
        public static IReadOnlyCollection<int> AsSafeReadOnlyDigitList(this int number)
        {
            return AsSafeReadOnlyDigitList((long)number);
        }

        /// <summary>
        /// Safe any.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// the result of the expression
        /// </returns>
        public static bool SafeAny<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            var safeList = list.SafeReadOnlyList();
            return safeList.Any(expression);
        }

        /// <summary>
        /// Safe all.
        /// be careful All returns true for empty lists
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// the result of the expression
        /// </returns>
        public static bool SafeAll<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            var safeList = list.SafeReadOnlyList();
            return safeList.All(expression);
        }

        /// <summary>
        /// Safe where.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// the result of the expression
        /// </returns>
        public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            var safeList = list.SafeReadOnlyList();
            return safeList.Where(expression);
        }

        /// <summary>
        /// Safe count.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// the result of the expression
        /// </returns>
        public static int SafeCount<T>(this IEnumerable<T> list, Func<T, bool> expression)
        {
            var safeList = list.SafeReadOnlyList();
            return safeList.Count(expression);
        }

        /// <summary>
        /// For each, to safe list and conducts the action
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>(nameof(action));

            var items = collection.AsSafeList();
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// For each, to safe list and conducts the action
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>(nameof(action));

            var items = collection.SafeReadOnlyList();
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// For any item that matches the condition do the action.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="matchCondition">match condition.</param>
        /// <param name="doAction">do action.</param>
        public static void ForAny<T>(this IEnumerable<T> list, Func<T, bool> matchCondition, Action<T> doAction)
        {
            It.IsNull(matchCondition)
                .AsGuard<ArgumentNullException>(nameof(matchCondition));
            It.IsNull(doAction)
                .AsGuard<ArgumentNullException>(nameof(doAction));

            var safeList = list.SafeReadOnlyList();
            safeList.ForEach(x =>
            {
                if (matchCondition(x))
                {
                    doAction(x);
                }
            });
        }

        /// <summary>
        /// Safe list, the private implementation, null coalescing
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe list
        /// </returns>
        private static List<T> SafeList<T>(this IEnumerable<T> list)
        {
            return new List<T>(list ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Safes the read only list.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe readonly list
        /// </returns>
        private static IReadOnlyCollection<T> SafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return list.SafeList();
        }
    }
}
