using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record PiecePlacedMessage : Message
    {
        public uint LinesCleared;
        public uint CurrentCombo;
        public uint CurrentBackToBack;
        public string PieceType;
        public bool WasAllClear;
        public bool WasSpin;
        public bool WasSpinMini;
    }
}