namespace Blockstacker.Gameplay.Randomizers
{
    public interface IRandomizer
    {
        string GetNextPiece();
        void Reset(int newSeed);
    }
}