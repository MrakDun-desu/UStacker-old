namespace Blockstacker.Gameplay.Communication
{
    public record ScoreChangedMessage : Message
    {
        public long Score;
    }
}