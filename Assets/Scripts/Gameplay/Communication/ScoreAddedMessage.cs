namespace UStacker.Gameplay.Communication
{
    public record ScoreAddedMessage : MidgameMessage
    {
        public readonly long ScoreAddition;

        public ScoreAddedMessage(long scoreAddition, double time) : base(time)
        {
            ScoreAddition = scoreAddition;
        }
    }
}