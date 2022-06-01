using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CountPerBagRandomizer : IRandomizer
    {
        private readonly int[] _availableValues;
        private readonly int _count;
        private readonly List<int> _currentValues;

        public CountPerBagRandomizer(int range, int count = 1)
        {
            _availableValues = new int[range];
            for (var i = 0; i < range; i++) _availableValues[i] = i;
            _count = count;
            _currentValues = new List<int>();
            InitializeCurrentPieces();
        }

        public int GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = Random.Range(0, _currentValues.Count);
            var nextValue = _currentValues[nextIndex];
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        private void InitializeCurrentPieces()
        {
            for (var i = 0; i < _count; i++) _currentValues.AddRange(_availableValues);
        }
    }
}