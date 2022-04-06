namespace Blockstacker.Gameplay.Randomizers
{
    public class ClassicRandomizer : IRandomizer
    {
        private readonly int _range;
        private int _lastValue;

        public ClassicRandomizer(int range)
        {
            _range = range;
            _lastValue = range;
        }

        public int GetNextPiece()
        {
            var nextValue = UnityEngine.Random.Range(0, _range);
            if (nextValue == _lastValue) {
                nextValue = UnityEngine.Random.Range(0, _range);
            }
            _lastValue = nextValue;
            return nextValue;
        }
    }
}