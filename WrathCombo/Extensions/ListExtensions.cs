using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WrathCombo.Extensions
{
    internal static class ListExtensions
    {
        private static readonly Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static void SwapValues<T>(this T[] source, long index1, long index2)
        {
            T temp = source[index1];
            source[index1] = source[index2];
            source[index2] = temp;
        }

        // Find the first object fulfilling predicate's criteria in the given list, if one exists.
        // Returns true if an object is found, false otherwise.
        public static bool FindFirst<T>(this IEnumerable<T> array, Predicate<T> predicate, [NotNullWhen(true)] out T? result)
        {
            foreach (var obj in array)
            {
                if (predicate(obj))
                {
                    result = obj!;
                    return true;
                }
            }

            result = default;
            return false;
        }

        // Find the first occurrence of needle in the given list and return the value contained in the list in result.
        // Returns true if an object is found, false otherwise.
        public static bool FindFirst<T>(this IEnumerable<T> array, T needle, [NotNullWhen(true)] out T? result) where T : notnull
        {
            foreach (var obj in array)
            {
                if (obj.Equals(needle))
                {
                    result = obj;
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
