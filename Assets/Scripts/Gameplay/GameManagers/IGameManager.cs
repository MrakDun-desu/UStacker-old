using Blockstacker.Gameplay.Communication;

namespace Blockstacker.Gameplay.GameManagers
{
    public interface IGameManager
    {
        void Initialize(uint startingLevel, MediatorSO mediator);
    }
}