
/************************************
AdvancedRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class AdvancedRandomizer : IRandomizer
    {
        private const string S = "s";
        private const string Z = "z";

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

        private readonly string[] _firstTryValues =
        {
            "i",
            "t",
            "l",
            "j"
        };

        private readonly string[] _lastPieces = {Z, S, Z, S};
        private readonly Random _random = new();
        private int _queueIndex;
        private bool _started;

        public AdvancedRandomizer(IEnumerable<string> availablePieces)
        {
            var availableArray = availablePieces as string[] ?? availablePieces.ToArray();
            _availableValues = _availableValues.Filter(availableArray).ToArray();
            _firstTryValues = _firstTryValues.Filter(availableArray).ToArray();
        }

        public string GetNextPiece()
        {
            const int TRY_COUNT = 6;
            int nextIndex;
            string nextValue;

            if (!_started)
            {
                nextIndex = _random.NextInt(_firstTryValues.Length);
                nextValue = _firstTryValues[nextIndex];

                _started = true;
            }
            else
            {
                nextIndex = _random.NextInt(_availableValues.Length);
                nextValue = _availableValues[nextIndex];

                for (var i = 0; i < TRY_COUNT; i++)
                {
                    if (!_lastPieces.Contains(nextValue)) break;

                    nextIndex = _random.NextInt(_availableValues.Length);
                    nextValue = _availableValues[nextIndex];
                }
            }

            _lastPieces[_queueIndex] = nextValue;
            _queueIndex = (_queueIndex + 1) % _lastPieces.Length;

            return _availableValues[nextIndex];
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _started = false;
            _queueIndex = 0;
            for (var i = 0; i < _lastPieces.Length; i++)
                _lastPieces[i] = i % 2 == 0 ? Z : S;
        }
    }
}
/************************************
end AdvancedRandomizer.cs
*************************************/
