using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record LinesDroppedMessage : Message
    {
        public uint Count;
        public bool WasHardDrop = false;
    }
}