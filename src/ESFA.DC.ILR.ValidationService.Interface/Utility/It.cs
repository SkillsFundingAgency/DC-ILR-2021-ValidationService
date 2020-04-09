using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    /// <summary>
    /// class encapsulating many type and content evaluation routines
    /// </summary>
    public static class It
    {
        public static bool IsType<T>(object value)
            where T : class
        {
            // FIX: not sure if 'is nested' is right
            return Has(value) && (value is T || typeof(T).IsNested);
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <typeparam name="T">of reference type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsNull<T>(T value)
            where T : class
        {
            return value == null;
        }

        /// <summary>
        /// Determines whether [has] [the specified value].
        /// </summary>
        /// <typeparam name="T">of reference type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool Has<T>(T value)
            where T : class
        {
            return value != null;
        }

        /// <summary>
        /// Determines whether [has] [the specified value].
        /// </summary>
        /// <typeparam name="T">of nullable value type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool Has<T>(T? value)
            where T : struct
        {
            return value != null;
        }

        /// <summary>
        /// Determines whether [has] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool Has(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Determines whether the specified values has values.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>true or false</returns>
        public static bool HasValues<T>(IEnumerable<T> values)
        {
            return Has(values) && values.Any();
        }

        /// <summary>
        /// Determines whether the specified values is empty.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty<T>(IEnumerable<T> values)
        {
            return IsNull(values) || !values.Any();
        }

        /// <summary>
        /// Determines whether the specified value is empty.
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty<T>(T? value)
            where T : struct
        {
            return value == null || !value.HasValue;
        }

        /// <summary>
        /// Determines whether [is usable unique identifier] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsUsable(Guid value)
        {
            return !value.Equals(Guid.Empty);
        }

        /// <summary>
        /// Determines whether [is in range] [the specified source].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>true or false</returns>
        public static bool IsInRange<T>(T source, params T[] target)
            where T : IComparable
        {
            return target != null ? target.Contains(source) : false;
        }

        /// <summary>
        /// Determines whether [is in range] [the specified source].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>true or false</returns>
        public static bool IsInRange(string source, params string[] target)
        {
            return (target == null || !target.Any()) ? false : target.ToList().Contains(source);
        }

        /// <summary>
        /// Determines whether [is out of range] [the specified source].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if [is out of range] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOutOfRange(string source, params string[] target)
        {
            return !IsInRange(source, target);
        }

        /// <summary>
        /// Determines whether [is in range] [the specified source].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>true or false</returns>
        public static bool IsInRange<T>(T? source, params T[] target)
            where T : struct, IComparable, IFormattable
        {
            return (target == null || !target.Any() || !source.HasValue) ? false : target.ToList().Contains(source.Value);
        }

        /// <summary>
        /// Determines whether [is out of range] [the specified source].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if [is out of range] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOutOfRange<T>(T? source, params T[] target)
            where T : struct, IComparable, IFormattable
        {
            return !IsInRange(source, target);
        }

        /// <summary>
        /// Determines whether [is in range] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>
        /// true or false
        /// </returns>
        public static bool IsBetween(int candidate, int min, int max, bool includeBoundaries = true)
        {
            return includeBoundaries
                ? candidate >= min && candidate <= max
                : candidate > min && candidate < max;
        }

        /// <summary>
        /// Determines whether the specified candidate is between.
        /// still semantically correct even though some dates do not conform to the (max > min) principal
        /// (max < min).AsGuard{ArgumentOutOfRangeException}(nameof(max));
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is between; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetween(DateTime candidate, DateTime min, DateTime max, bool includeBoundaries = true) =>
            includeBoundaries
                ? candidate >= min && candidate <= max
                : candidate > min && candidate < max;
    }
}
