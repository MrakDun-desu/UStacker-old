namespace Blockstacker.Gameplay.Communication
{
    public class PiecePlacedMessage : IMessage
    {
        public int linesCleared;
        public int cheeseLinesCleared;
        public bool wasSpin;
        public bool wasSpinMini;
        public bool wasAllClear;
        public bool wasColorClear;
    }
}