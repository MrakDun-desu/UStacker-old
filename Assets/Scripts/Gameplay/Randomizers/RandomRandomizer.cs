
/************************************
RandomRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
    {
        private readonly string[] _availableValues =
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
            _availableValues = _availableValues.Filter(availablePieces).ToArray();
        }

        public string GetNextPiece()
        {
            return _availableValues[_random.NextInt(_availableValues.Length)];
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
        }
    }
}
/************************************
end RandomRandomizer.cs
*************************************/
