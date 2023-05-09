
/************************************
IMidgameMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.Gameplay.Communication
{
    public interface IMidgameMessage : IMessage
    {
        public double Time { get; }
    }
}
/************************************
end IMidgameMessage.cs
*************************************/
