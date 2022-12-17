namespace UStacker.Gameplay.Communication
{
    public record CountdownTickedMessage : Message
    {
        public readonly uint RemainingTicks;

        public CountdownTickedMessage(uint remainingTicks)
        {
            RemainingTicks = remainingTicks;
        }
    }
}