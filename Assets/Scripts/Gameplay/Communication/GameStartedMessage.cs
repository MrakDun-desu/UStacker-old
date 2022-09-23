namespace Blockstacker.Gameplay.Communication
{
    public record GameStartedMessage : Message
    {
        public readonly int Seed;

        public GameStartedMessage(int seed)
        {
            Seed = seed;
        }
    }
}