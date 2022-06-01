namespace Blockstacker.Gameplay.Communication
{
    public record PiecePlacedMessage : Message
    {
        public uint LinesCleared;
        public bool WasAllClear;
    }
}