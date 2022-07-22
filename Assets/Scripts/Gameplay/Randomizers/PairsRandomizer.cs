using System;

namespace Blockstacker.Gameplay.Randomizers
{
    public class PairsRandomizer : IRandomizer
    {
        private readonly int _range;
        private readonly Random _random;
        private int _lastValue;
        private bool _shouldChange = true;

        public PairsRandomizer(int range, int seed)
        {
            _range = range;
            _random = new Random(seed);
            _lastValue = range;
        }

        public int GetNextPiece()
        {
            if (_shouldChange)
                _lastValue = _random.Next(0, _range);

            _shouldChange = !_shouldChange;
            return _lastValue;
        }
    }
}