using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Randomizers
{
    public class StrideRandomizer : IRandomizer
    {
        private readonly int[] _availableValues;
        private readonly List<int> _currentValues;
        private readonly Random _random;
        private int _ignoreSzoFor = 2;

        public StrideRandomizer(int range, int seed)
        {
            _random = new Random(seed);
            _availableValues = new int[range];
            for (var i = 0; i < range; i++) 
                _availableValues[i] = i;
            _currentValues = new List<int>();
            InitializeCurrentPieces();
        }

        public int GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.Next(0, _currentValues.Count);
            var nextValue = _currentValues[nextIndex];

            if (_ignoreSzoFor > 0)
            {
                while (nextValue > 3) // undesired pieces are above 3 (o, s, z)
                {
                    nextIndex = _random.Next(0, _currentValues.Count);
                    nextValue = _currentValues[nextIndex];
                }

                _ignoreSzoFor--;
            }
            
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        private void InitializeCurrentPieces()
        {
            _currentValues.AddRange(_availableValues);
        }
    }
}