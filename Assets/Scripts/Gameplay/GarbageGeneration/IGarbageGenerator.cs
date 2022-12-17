using Blockstacker.Gameplay.Communication;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public interface IGarbageGenerator
    {
        void ResetState(int seed);

        void GenerateGarbage(uint amount, PiecePlacedMessage message = null);
    }
}