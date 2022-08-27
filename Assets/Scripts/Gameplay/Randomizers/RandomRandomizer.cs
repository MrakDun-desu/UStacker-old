using System;
using System.Collections.Generic;
using Blockstacker.Common.Extensions;

namespace Blockstacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
    {
        private readonly List<string> _availableValues = new ()
        {
            "i",
            "t",
            "o",
            "l",
            "j",
            "s",
            "z",
        };
        private readonly Random _random;

        public RandomRandomizer(IEnumerable<string> availablePieces, int seed)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _random = new Random(seed);
        }

        public string GetNextPiece()
        {
            return _availableValues[_random.Next(0, _availableValues.Count)];
        }
    }
}