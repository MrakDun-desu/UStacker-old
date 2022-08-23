namespace Blockstacker.Gameplay.Communication
{
    public record GameStartedMessage : Message
    {
        public int Seed;
    }
}