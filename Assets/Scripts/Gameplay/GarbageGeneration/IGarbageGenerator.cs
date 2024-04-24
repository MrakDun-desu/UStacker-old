
/************************************
IGarbageGenerator.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Gameplay.Communication;

namespace UStacker.Gameplay.GarbageGeneration
{
    public interface IGarbageGenerator
    {
        void ResetState(ulong seed);

        void GenerateGarbage(uint amount, PiecePlacedMessage message);
    }
}
/************************************
end IGarbageGenerator.cs
*************************************/
