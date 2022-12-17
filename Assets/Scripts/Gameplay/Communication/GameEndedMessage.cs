namespace UStacker.Gameplay.Communication
{
    public record GameEndedMessage : Message
    {
        public readonly double EndTime;

        public GameEndedMessage(double endTime)
        {
            EndTime = endTime;
        }
    }
}