namespace Blockstacker.Gameplay.Communication
{
    public record LinesDroppedMessage : Message
    {
        public uint Count;
        public bool WasHardDrop = false;
    }
}