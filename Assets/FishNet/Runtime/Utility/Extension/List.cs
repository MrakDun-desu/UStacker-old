using System.Collections.Generic;
using FishNet.Documenting;

namespace FishNet.Utility.Extension
{
    [APIExclude]
    public static class ListFN
    {
        /// <summary>
        ///     Adds a value to the list only if the value does not already exist.
        /// </summary>
        /// <param name="lst">Collection being added to.</param>
        /// <param name="value">Value to add.</param>
        public static void AddUnique<T>(this List<T> lst, T value)
        {
            if (!lst.Contains(value))
                lst.Add(value);
        }
    }
}