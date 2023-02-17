namespace UStacker.Gameplay.Communication
{
    public readonly struct ScoreAddedMessage : IMidgameMessage
    {
        public readonly long ScoreAddition;
        public double Time { get; }

        public ScoreAddedMessage(long scoreAddition, double time)
        {
            Time = time;
            ScoreAddition = scoreAddition;
        }
    }
}