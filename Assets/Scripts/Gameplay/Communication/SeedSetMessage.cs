
/************************************
SeedSetMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
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
/************************************
end SeedSetMessage.cs
*************************************/
