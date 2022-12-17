using UStacker.Gameplay.Communication;

namespace UStacker.Gameplay.GarbageGeneration
{
    public interface IGarbageGenerator
    {
        void ResetState(int seed);

        void GenerateGarbage(uint amount, PiecePlacedMessage message = null);
    }
}