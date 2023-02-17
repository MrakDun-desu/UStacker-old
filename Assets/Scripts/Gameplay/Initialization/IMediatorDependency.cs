using UStacker.Gameplay.Communication;

namespace UStacker.Gameplay.Initialization
{
    public interface IMediatorDependency
    {
        Mediator Mediator { set; }
    }
}