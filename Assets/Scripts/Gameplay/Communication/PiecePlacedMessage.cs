using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record PiecePlacedMessage : Message
    {
        public uint LinesCleared;
        public bool WasAllClear;
        public bool WasSpin;
        public bool WasSpinMini;
    }
}