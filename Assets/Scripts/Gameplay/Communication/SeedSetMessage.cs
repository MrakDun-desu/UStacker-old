namespace UStacker.Gameplay.Communication
{
    public readonly struct SeedSetMessage : IMessage
    {
        public readonly ulong Seed;

        public SeedSetMessage(ulong seed)
        {
            Seed = seed;
        }
    }
}