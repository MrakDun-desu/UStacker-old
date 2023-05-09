
/************************************
ClassicRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class ClassicRandomizer : IRandomizer
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

        private int _lastIndex;

        public ClassicRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces).ToArray();
        }

        public string GetNextPiece()
        {
            var nextIndex = _random.NextInt(_availableValues.Length);
            if (nextIndex == _lastIndex) nextIndex = _random.NextInt(_availableValues.Length);
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
/************************************
end ClassicRandomizer.cs
*************************************/
