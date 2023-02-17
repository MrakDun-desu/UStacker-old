namespace UStacker.Gameplay.Communication
{
    public readonly struct CountdownTickedMessage : IMessage
    {
        public readonly uint RemainingTicks;

        public CountdownTickedMessage(uint remainingTicks)
        {
            RemainingTicks = remainingTicks;
        }
    }
}