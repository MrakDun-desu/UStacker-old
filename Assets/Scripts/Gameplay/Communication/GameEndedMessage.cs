namespace Blockstacker.Gameplay.Communication
{
    public record GameEndedMessage : Message
    {
        public double EndTime;
    }
}