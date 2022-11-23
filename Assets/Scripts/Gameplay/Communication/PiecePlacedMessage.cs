using System;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record PiecePlacedMessage : MidgameMessage
    {
        public readonly uint LinesCleared;
        public readonly uint GarbageLinesCleared;
        public readonly uint CurrentCombo;
        public readonly uint CurrentBackToBack;
        public readonly string PieceType;
        public readonly bool WasAllClear;
        public readonly bool WasSpin;
        public readonly bool WasSpinMini;
        public readonly bool WasSpinRaw;
        public readonly bool WasSpinMiniRaw;
        public readonly bool BrokenCombo;
        public readonly bool BrokenBackToBack;
        public bool WasBtbClear => WasSpin || WasSpinMini || LinesCleared > 3;

        public PiecePlacedMessage(uint linesCleared, uint garbageLinesCleared, uint currentCombo,
            uint currentBackToBack, string pieceType, bool wasAllClear, bool wasSpin, bool wasSpinMini, bool wasSpinRaw,
            bool wasSpinMiniRaw, bool brokenCombo, bool brokenBackToBack, double time) : base(time)
        {
            LinesCleared = linesCleared;
            GarbageLinesCleared = garbageLinesCleared;
            CurrentCombo = currentCombo;
            CurrentBackToBack = currentBackToBack;
            PieceType = pieceType;
            WasAllClear = wasAllClear;
            WasSpin = wasSpin;
            WasSpinMini = wasSpinMini;
            WasSpinRaw = wasSpinRaw;
            WasSpinMiniRaw = wasSpinMiniRaw;
            BrokenCombo = brokenCombo;
            BrokenBackToBack = brokenBackToBack;
        }

        public PiecePlacedMessage() : base(0)
        {
        }

        public PiecePlacedMessage(uint linesCleared, uint garbageLinesCleared, bool wasAllClear, double time,
            bool wasSpin, bool wasSpinMini, bool wasSpinRaw, bool wasSpinMiniRaw, string pieceType) : base(time)
        {
            LinesCleared = linesCleared;
            GarbageLinesCleared = garbageLinesCleared;
            WasAllClear = wasAllClear;
            WasSpin = wasSpin;
            WasSpinMini = wasSpinMini;
            WasSpinRaw = wasSpinRaw;
            WasSpinMiniRaw = wasSpinMiniRaw;
            PieceType = pieceType;
        }
    }
}