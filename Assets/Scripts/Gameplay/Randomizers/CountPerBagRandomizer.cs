
/************************************
CountPerBagRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections.Generic;
using System.Linq;
using UStacker.Common;
using UStacker.Common.Extensions;

namespace UStacker.Gameplay.Randomizers
{
    public class CountPerBagRandomizer : IRandomizer
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

        private readonly int _count;
        private readonly List<string> _currentValues = new();
        private readonly Random _random = new();

        public CountPerBagRandomizer(IEnumerable<string> availablePieces, int count = 1)
        {
            _availableValues = _availableValues.Filter(availablePieces).ToArray();
            _count = count;
        }

        public string GetNextPiece()
        {
            if (_currentValues.Count == 0) InitializeCurrentValues();
            var nextIndex = _random.NextInt(_currentValues.Count);
            var nextValue = _currentValues[nextIndex];
            _currentValues.RemoveAt(nextIndex);
            return nextValue;
        }

        public void Reset(ulong newSeed)
        {
            _random.State = newSeed;
            _currentValues.Clear();
            InitializeCurrentValues();
        }

        private void InitializeCurrentValues()
        {
            for (var i = 0; i < _count; i++)
                _currentValues.AddRange(_availableValues);
        }
    }
}
/************************************
end CountPerBagRandomizer.cs
*************************************/
