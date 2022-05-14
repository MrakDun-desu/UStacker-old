namespace Blockstacker.Gameplay.Randomizers
{
    public class PairsRandomizer : IRandomizer
    {
        private readonly int _range;
        private int _lastValue;
        private bool _shouldChange = true;

        public PairsRandomizer(int range)
        {
            _range = range;
            _lastValue = range;
        }

        public int GetNextPiece()
        {
            if (_shouldChange)
                _lastValue = UnityEngine.Random.Range(0, _range);

            _shouldChange = !_shouldChange;
            return _lastValue;
        }
    }
}