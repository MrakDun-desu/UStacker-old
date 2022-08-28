using System;
using System.Collections.Generic;
using Blockstacker.Common.Extensions;

namespace Blockstacker.Gameplay.Randomizers
{
    public class ClassicRandomizer : IRandomizer
    {
        private readonly List<string> _availableValues = new()
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
        private int _lastIndex;

        public ClassicRandomizer(IEnumerable<string> availablePieces, int seed)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _random = new Random(seed);
            _lastIndex = -1;
        }

        public string GetNextPiece()
        {
            var nextIndex = _random.Next(0, _availableValues.Count);
            if (nextIndex == _lastIndex) nextIndex = _random.Next(0, _availableValues.Count);
            _lastIndex = nextIndex;
            return _availableValues[nextIndex];
        }
    }
}