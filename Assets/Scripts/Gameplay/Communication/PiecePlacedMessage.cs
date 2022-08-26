using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record PiecePlacedMessage : MidgameMessage
    {
        public uint LinesCleared;
        public uint GarbageLinesCleared;
        public uint CurrentCombo;
        public uint CurrentBackToBack;
        public string PieceType;
        public bool WasAllClear;
        public bool WasSpin;
        public bool WasSpinMini;
        public bool WasSpinRaw;
        public bool WasSpinMiniRaw;
        public bool BrokenCombo;
        public bool BrokenBackToBack;
    }
}