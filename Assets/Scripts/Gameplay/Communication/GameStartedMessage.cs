namespace UStacker.Gameplay.Communication
{
    public record GameStartedMessage : Message
    {
        public readonly ulong Seed;

        public GameStartedMessage(ulong seed)
        {
            Seed = seed;
        }
    }
}