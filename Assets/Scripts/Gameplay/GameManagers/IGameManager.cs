using UStacker.Gameplay.Communication;

namespace UStacker.Gameplay.GameManagers
{
    public interface IGameManager
    {
        void Initialize(string startingLevel, Mediator mediator);
        void Delete();
    }
}