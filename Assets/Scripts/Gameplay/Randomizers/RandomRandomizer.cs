using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class RandomRandomizer : IRandomizer
    {
        private readonly int _range;

        public RandomRandomizer(int range)
        {
            _range = range;
        }

        public int GetNextPiece()
        {
            var nextValue = Random.Range(0, _range);
            return nextValue;
        }
    }
}