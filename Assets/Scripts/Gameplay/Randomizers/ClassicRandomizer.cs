using System.Collections.Generic;
using UStacker.Common.Extensions;
using UStacker.Common;

namespace UStacker.Gameplay.Randomizers
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
            "z"
        };
        private int _lastIndex;

        private readonly Random _random = new();

        public ClassicRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            _lastIndex = -1;
        }

        public string GetNextPiece()
        {
            var nextIndex = _random.NextInt(_availableValues.Count);
            if (nextIndex == _lastIndex) nextIndex = _random.NextInt(_availableValues.Count);
            _lastIndex = nextIndex;
            return _availableValues[nextIndex];
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _lastIndex = -1;
        }
    }
}