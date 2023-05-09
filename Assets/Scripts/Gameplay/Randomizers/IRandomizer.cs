
/************************************
IRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.Gameplay.Randomizers
{
    public interface IRandomizer
    {
        string GetNextPiece();

        void Reset(ulong newSeed);
    }
}
/************************************
end IRandomizer.cs
*************************************/
