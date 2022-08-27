using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blockstacker.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool TryGetRandomElement<T>(this IEnumerable<T> enumerable, out T element)
        {
            element = default;
            var list = enumerable as T[] ?? enumerable.ToArray();
            if (list.Length == 0) return false;
            var index = Random.Range(0, list.Length);
            element = list[index];
            return true;
        }

        public static List<T> Filter<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
        {
            var array = enumerable.ToArray();
            var outList = other.ToList();
            for (var i = 0; i < array.Length; i++)
            {
                if (array.Contains(array[i])) continue;
                
                outList.RemoveAt(i);
                i--;
            }

            return outList;
        }
    }
}