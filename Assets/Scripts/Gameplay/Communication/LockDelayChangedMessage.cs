namespace Blockstacker.Gameplay.Communication
{
    public record LockDelayChangedMessage : Message
    {
        public double LockDelay;
    }
}