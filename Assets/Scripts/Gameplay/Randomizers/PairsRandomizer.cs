
/************************************
PairsRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class PairsRandomizer : IRandomizer
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
        private bool _shouldChange = true;

        public PairsRandomizer(IEnumerable<string> availablePieces)
        {
            _availableValues = _availableValues.Filter(availablePieces).ToArray();
        }

        public string GetNextPiece()
        {
            if (_shouldChange)
                _lastIndex = _random.NextInt(_availableValues.Length);

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
/************************************
end PairsRandomizer.cs
*************************************/
