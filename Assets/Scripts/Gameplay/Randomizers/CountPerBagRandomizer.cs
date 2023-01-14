using System.Collections.Generic;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
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
        private readonly Random _random = new();

        public CountPerBagRandomizer(IEnumerable<string> availablePieces, int count = 1)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _count = count;
            InitializeCurrentPieces();
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.NextInt(_currentValues.Count);
            var nextValue = _currentValues[nextIndex];
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
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