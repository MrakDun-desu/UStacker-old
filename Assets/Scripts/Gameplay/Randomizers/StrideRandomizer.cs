using UStacker.Common;
using System.Collections.Generic;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class StrideRandomizer : IRandomizer
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
        private readonly List<string> _currentValues = new();
        private int _ignoreSzoFor = 2;
        private readonly Random _random = new();

        public StrideRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            InitializeCurrentPieces();
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.NextInt(_currentValues.Count);
            var nextValue = _currentValues[nextIndex];

            if (_ignoreSzoFor > 0)
            {
                while (nextValue is "s" or "z" or "o")
                {
                    nextIndex = _random.NextInt(_currentValues.Count);
                    nextValue = _currentValues[nextIndex];
                }

                _ignoreSzoFor--;
            }

            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _currentValues.Clear();
            InitializeCurrentPieces();
            _ignoreSzoFor = 2;
        }

        private void InitializeCurrentPieces()
        {
            _currentValues.AddRange(_availableValues);
        }
    }
}