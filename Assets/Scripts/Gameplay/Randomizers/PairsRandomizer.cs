using System.Collections.Generic;
using UStacker.Common.Extensions;
using UStacker.Common;

namespace UStacker.Gameplay.Randomizers
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
            "z"
        };
        private int _lastIndex;
        private readonly Random _random = new();
        private bool _shouldChange = true;

        public PairsRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces);
        }

        public string GetNextPiece()
        {
            if (_shouldChange)
                _lastIndex = _random.NextInt(_availableValues.Count);

            _shouldChange = !_shouldChange;
            return _availableValues[_lastIndex];
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _shouldChange = true;
        }
    }
}