﻿using System;
using System.Collections.Generic;

namespace FishNet.Utility.Extension
{
    public static class CollectionFN
    {
        /// <summary>
        ///     Random for shuffling.
        /// </summary>
        private static readonly Random _random = new();

        /// <summary>
        ///     Shuffle based on Fisher-Yates shuffle.
        ///     https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        ///     https://stackoverflow.com/questions/273313/randomize-a-listt
        /// </summary>
        public static void Shuffle<T>(this IList<T> lst)
        {
            var n = lst.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
                var value = lst[k];
                lst[k] = lst[n];
                lst[n] = value;
            }
        }
    }
}