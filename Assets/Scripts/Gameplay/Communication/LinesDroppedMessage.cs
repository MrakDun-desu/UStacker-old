namespace Blockstacker.Gameplay.Communication
{
    public class LinesDroppedMessage : IMessage
    {
        public int count;
        public bool wasHardDrop = false;
    }
}