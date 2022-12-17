using System;
using System.Collections.Generic;
using Blockstacker.Common.Extensions;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CountPerBagRandomizer : IRandomizer
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

        private readonly int _count;
        private readonly List<string> _currentValues = new();
        private Random _random;

        public CountPerBagRandomizer(IEnumerable<string> availablePieces, int seed, int count = 1)
        {
            _availableValues = _availableValues.Filter(availablePieces);

            _random = new Random(seed);
            _count = count;
            InitializeCurrentPieces();
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.Next(0, _currentValues.Count);
            var nextValue = _currentValues[nextIndex];
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(int newSeed)
        {
            _random = new Random(newSeed);
            _currentValues.Clear();
            InitializeCurrentPieces();
        }

        private void InitializeCurrentPieces()
        {
            for (var i = 0; i < _count; i++)
                _currentValues.AddRange(_availableValues);
        }
    }
}