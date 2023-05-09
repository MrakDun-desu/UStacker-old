
/************************************
StrideRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class StrideRandomizer : IRandomizer
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

        private readonly List<string> _currentValues = new();

        private readonly string[] _firstTryValues =
        {
            "i",
            "t",
            "l",
            "j"
        };

        private readonly Random _random = new();
        private bool _started;

        public StrideRandomizer(IEnumerable<string> availablePieces)
        {
            var availableArray = availablePieces as string[] ?? availablePieces.ToArray();
            _availableValues = _availableValues.Filter(availableArray).ToArray();
            _firstTryValues = _firstTryValues.Filter(availableArray).ToArray();
            InitializeCurrentValues();
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentValues();

            int nextIndex;
            string nextValue;

            if (!_started)
            {
                nextIndex = _random.NextInt(_firstTryValues.Length);
                nextValue = _firstTryValues[nextIndex];

                nextIndex = _currentValues.IndexOf(nextValue);
                _started = true;
            }
            else
            {
                nextIndex = _random.NextInt(_currentValues.Count);
                nextValue = _currentValues[nextIndex];
            }

            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _currentValues.Clear();
            InitializeCurrentValues();
            _started = false;
        }

        private void InitializeCurrentValues()
        {
            _currentValues.AddRange(_availableValues);
        }
    }
}
/************************************
end StrideRandomizer.cs
*************************************/
