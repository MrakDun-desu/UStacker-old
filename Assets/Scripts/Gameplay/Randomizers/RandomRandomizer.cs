using System;

namespace Blockstacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
    {
        private readonly int _range;
        private readonly Random _random;

        public RandomRandomizer(int range, int seed)
        {
            _range = range;
            _random = new Random(seed);
        }

        public int GetNextPiece()
        {
            return _random.Next(0, _range);
        }
    }
}