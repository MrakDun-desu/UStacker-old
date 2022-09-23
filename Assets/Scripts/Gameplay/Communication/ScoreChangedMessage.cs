namespace Blockstacker.Gameplay.Communication
{
    public record ScoreChangedMessage : MidgameMessage
    {
        public readonly long Score;

        public ScoreChangedMessage(long score, double time) : base(time)
        {
            Score = score;
        }
    }
}