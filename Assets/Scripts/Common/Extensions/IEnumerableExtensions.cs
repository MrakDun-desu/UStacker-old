
/************************************
IEnumerableExtensions.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;

namespace UStacker.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool TryGetRandomElement<T>(this IEnumerable<T> enumerable, out T element)
        {
            element = default;
            var list = enumerable as T[] ?? enumerable.ToArray();
            if (list.Length == 0) return false;
            var index = UnityEngine.Random.Range(0, list.Length);
            element = list[index];
            return true;
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, IEnumerable<T> filter)
        {
            var filterArray = filter.ToArray();
            var outList = enumerable.ToList();
            for (var i = 0; i < outList.Count; i++)
            {
                if (filterArray.Contains(outList[i])) continue;

                outList.RemoveAt(i);
                i--;
            }

            return outList;
        }
    }
}
/************************************
end IEnumerableExtensions.cs
*************************************/
