using System;
using System.Collections.Generic;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
    {
        private readonly List<string> _availableValues = new()
        {
            "i",
            "t",
            "o",
            "l",
            "j",
            "s",
            "z"
        };
        private Random _random;

        public RandomRandomizer(IEnumerable<string> availablePieces, int seed)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _random = new Random(seed);
        }

        public string GetNextPiece()
        {
            return _availableValues[_random.Next(0, _availableValues.Count)];
        }

        public void Reset(int newSeed)
        {
            _random = new Random(newSeed);
        }
    }
}