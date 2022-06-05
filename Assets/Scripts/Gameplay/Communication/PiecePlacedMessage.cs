using System;
using Blockstacker.Common.Enums;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record PiecePlacedMessage : Message
    {
        public uint LinesCleared;
        public PieceType PieceType;
        public bool WasAllClear;
        public bool WasSpin;
        public bool WasSpinMini;
    }
}