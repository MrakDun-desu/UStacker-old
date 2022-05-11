namespace Blockstacker.Gameplay.Communication
{
    public class PiecePlacedMessage : Message
    {
        public uint LinesCleared;
        public bool WasAllClear;
    }
}