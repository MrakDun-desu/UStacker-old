using System;

namespace Blockstacker.Gameplay.Randomizers
{
    public class ClassicRandomizer : IRandomizer
    {
        private readonly int _range;
        private readonly Random _random;
        private int _lastValue;

        public ClassicRandomizer(int range, int seed)
        {
            _random = new Random(seed);
            _range = range;
            _lastValue = range;
        }

        public int GetNextPiece()
        {
            var nextValue = _random.Next(0, _range);
            if (nextValue == _lastValue) nextValue = _random.Next(0, _range);
            _lastValue = nextValue;
            return nextValue;
        }
    }
}