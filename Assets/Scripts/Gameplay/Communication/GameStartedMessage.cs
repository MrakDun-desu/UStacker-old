namespace UStacker.Gameplay.Communication
{
    public readonly struct GameStartedMessage : IMessage
    {
        public readonly ulong Seed;

        public GameStartedMessage(ulong seed)
        {
            Seed = seed;
        }
    }
}