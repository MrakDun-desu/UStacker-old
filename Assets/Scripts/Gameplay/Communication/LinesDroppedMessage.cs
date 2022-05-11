namespace Blockstacker.Gameplay.Communication
{
    public class LinesDroppedMessage : Message
    {
        public uint Count;
        public bool WasHardDrop = false;
    }
}