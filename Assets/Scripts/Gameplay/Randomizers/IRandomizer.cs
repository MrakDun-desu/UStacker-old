namespace UStacker.Gameplay.Randomizers
{
    public interface IRandomizer
    {
        string GetNextPiece();

        void Reset(ulong newSeed);
    }
}