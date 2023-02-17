namespace UStacker.Gameplay.Communication
{
    public interface IMidgameMessage : IMessage
    {
        public double Time { get; }
    }
}