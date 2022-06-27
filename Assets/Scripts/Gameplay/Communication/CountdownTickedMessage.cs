namespace Blockstacker.Gameplay.Communication
{
    public record CountdownTickedMessage : Message
    {
        public uint RemainingTicks;
    }
}