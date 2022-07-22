using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CountPerBagRandomizer : IRandomizer
    {
        private readonly int[] _availableValues;
        private readonly int _count;
        private readonly List<int> _currentValues;
        private readonly Random _random;

        public CountPerBagRandomizer(int range, int seed, int count = 1)
        {
            _random = new Random(seed);
            _availableValues = new int[range];
            for (var i = 0; i < range; i++) 
                _availableValues[i] = i;
            _count = count;
            _currentValues = new List<int>();
            InitializeCurrentPieces();
        }

        public int GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.Next(0, _currentValues.Count);
            var nextValue = _currentValues[nextIndex];
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        private void InitializeCurrentPieces()
        {
            for (var i = 0; i < _count; i++) 
                _currentValues.AddRange(_availableValues);
        }
    }
}