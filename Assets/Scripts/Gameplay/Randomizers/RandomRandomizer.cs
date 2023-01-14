using UStacker.Common;
using System.Collections.Generic;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
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

        private readonly Random _random = new();

        public RandomRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces);
        }

        public string GetNextPiece()
        {
            return _availableValues[_random.NextInt(_availableValues.Count)];
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
        }
    }
}