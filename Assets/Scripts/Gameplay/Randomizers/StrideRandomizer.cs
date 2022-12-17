using System;
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
        private Random _random;

        public StrideRandomizer(IEnumerable<string> availablePieces, int seed)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _random = new Random(seed);
            InitializeCurrentPieces();
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentPieces();
            var nextIndex = _random.Next(0, _currentValues.Count);
            var nextValue = _currentValues[nextIndex];

            if (_ignoreSzoFor > 0)
            {
                while (nextValue is "s" or "z" or "o")
                {
                    nextIndex = _random.Next(0, _currentValues.Count);
                    nextValue = _currentValues[nextIndex];
                }

                _ignoreSzoFor--;
            }

            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(int newSeed)
        {
            _random = new Random(newSeed);
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