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
        /// <summary>
        /// Determines whether [is] [the specified value].
        /// </summary>
        /// <typeparam name="T">of value type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsValueType<T>(object value)
            where T : IComparable
        {
            // FIX: not sure if 'is nested' is right
            return Has(value) && (value is T || typeof(T).IsNested);
        }

        public static bool IsType<T>(object value)
            where T : class
        {
            // FIX: not sure if 'is nested' is right
            return Has(value) && (value is T || typeof(T).IsNested);
        }

        /// <summary>
        /// Determines whether the specified value is not.
        /// </summary>
        /// <typeparam name="T">of reference type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsNotType<T>(object value)
            where T : class, IComparable
        {
            return !IsType<T>(value);
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
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
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
        /// Determines whether the specified value is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty(Guid value)
        {
            return !IsUsable(value);
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
        /// Determines whether [is usable integer] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parsedOut">The parsed out value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>true or false</returns>
        public static bool IsUsable(string value, out int parsedOut, int min = int.MinValue, int max = int.MaxValue)
        {
            return int.TryParse(value, out parsedOut) && IsBetween(parsedOut, min, max);
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
            var values = target.AsSafeReadOnlyList();
            return values.Contains(source);
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
        public static bool IsOutOfRange<T>(T source, params T[] target)
            where T : IComparable
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
        public static bool IsInRange(string source, params string[] target)
        {
            var values = target.AsSafeDistinctKeySet();
            return values.Contains(source);
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
            var values = target.AsSafeDistinctKeySet();
            return Has(source) && values.Contains(source.Value);
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
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is between; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetween(TimeSpan candidate, TimeSpan min, TimeSpan max, bool includeBoundaries = true)
        {
            (max < min)
                .AsGuard<ArgumentOutOfRangeException>(nameof(max));

            return includeBoundaries
                ? candidate >= min && candidate <= max
                : candidate > min && candidate < max;
        }

        /// <summary>
        /// Determines whether the specified candidate is between.
        /// out of range exception where max is less than min
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is between; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetween(DateTime candidate, DateTime min, DateTime max, bool includeBoundaries = true)
        {
            (max < min)
                .AsGuard<ArgumentOutOfRangeException>(nameof(max));

            return includeBoundaries
                ? candidate >= min && candidate <= max
                : candidate > min && candidate < max;
        }

        /// <summary>
        /// Determines whether [is out of range] [the specified candidate].
        /// out of range exception where max is less than min
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>
        /// true or false
        /// </returns>
        public static bool IsNotBetween(int candidate, int min, int max, bool includeBoundaries = true)
        {
            return !IsBetween(candidate, min, max, includeBoundaries);
        }

        /// <summary>
        /// Determines whether [has count of] [the specified values].
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="values">The values.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>true or false</returns>
        public static bool HasCountOf<T>(IEnumerable<T> values, int expectedCount)
        {
            var enumerable = values.AsSafeList();
            return enumerable.Count == expectedCount;
        }

        /// <summary>
        /// Determines whether the strings are the same.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>true or false</returns>
        public static bool IsTheSame(string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the strings are different.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>true or false</returns>
        public static bool IsDifferent(string a, string b)
        {
            return !IsTheSame(a, b);
        }

        /// <summary>
        /// Determines whether the specified old value is different for mthe new value.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        ///   <c>true</c> if the specified old value is different; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDifferent<T>(T oldValue, T newValue)
        {
            return !((Equals(oldValue, default(T)) && Equals(newValue, default(T))) || Equals(oldValue, newValue));
        }

        /// <summary>
        /// Determines whether the specified item is defined.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>true or false</returns>
        public static bool IsDefined<TEnum>(string item)
            where TEnum : struct, IComparable, IFormattable
        {
            return Enum.TryParse(item, true, out TEnum parsed);
        }
    }
}
