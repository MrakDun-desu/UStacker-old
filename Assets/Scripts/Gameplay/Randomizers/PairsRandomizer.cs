using System;
using System.Collections.Generic;
using Blockstacker.Common.Extensions;

namespace Blockstacker.Gameplay.Randomizers
{
    public class PairsRandomizer : IRandomizer
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
        private Random _random;
        private int _lastIndex;
        private bool _shouldChange = true;

        public PairsRandomizer(IEnumerable<string> availablePieces, int seed)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            
            _random = new Random(seed);
        }

        public string GetNextPiece()
        {
            if (_shouldChange)
                _lastIndex = _random.Next(0, _availableValues.Count);

            _shouldChange = !_shouldChange;
            return _availableValues[_lastIndex];
        }

        public void Reset(int newSeed)
        {
            _random = new Random(newSeed);
            _shouldChange = true;
        }
    }
}