
/************************************
PiecePlacedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct PiecePlacedMessage : IMidgameMessage
    {
        public readonly Vector2Int[] BlockPositions;
        public readonly bool BrokenBackToBack;
        public readonly bool BrokenCombo;
        public readonly uint CurrentBackToBack;
        public readonly uint CurrentCombo;
        public readonly uint GarbageLinesCleared;
        public readonly uint LinesCleared;
        public readonly string PieceType;
        public readonly Vector2Int TotalMovement;
        public readonly int TotalRotation;
        public readonly bool WasAllClear;
        public readonly bool WasSpin;
        public readonly bool WasSpinMini;
        [UsedImplicitly] public readonly bool WasSpinMiniRaw;
        [UsedImplicitly] public readonly bool WasSpinRaw;

        public PiecePlacedMessage(uint linesCleared, uint garbageLinesCleared, uint currentCombo,
            uint currentBackToBack, string pieceType, bool wasAllClear, bool wasSpin, bool wasSpinMini, bool wasSpinRaw,
            bool wasSpinMiniRaw, bool brokenCombo, bool brokenBackToBack, int totalRotation, Vector2Int totalMovement,
            Vector2Int[] blockPositions, double time)
        {
            Time = time;
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
            TotalRotation = totalRotation;
            TotalMovement = totalMovement;
            BlockPositions = blockPositions;
        }

        public bool WasBtbClear => WasSpin || WasSpinMini || LinesCleared > 3;
        public double Time { get; }
    }
}
/************************************
end PiecePlacedMessage.cs
*************************************/
