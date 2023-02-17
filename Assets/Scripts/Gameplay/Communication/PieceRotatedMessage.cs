using JetBrains.Annotations;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay.Communication
{
    public readonly struct PieceRotatedMessage : IMidgameMessage
    {
        [UsedImplicitly]
        public readonly RotationState EndRotation;
        [UsedImplicitly]
        public readonly string PieceType;
        [UsedImplicitly]
        public readonly RotationState StartRotation;
        public readonly bool WasSpin;
        public readonly bool WasSpinMini;
        [UsedImplicitly]
        public readonly bool WasSpinMiniRaw;
        [UsedImplicitly]
        public readonly bool WasSpinRaw;
        public double Time { get; }

        public PieceRotatedMessage(string pieceType, RotationState startRotation, RotationState endRotation,
            bool wasSpin, bool wasSpinMini, bool wasSpinRaw, bool wasSpinMiniRaw, double time)
        {
            Time = time;
            PieceType = pieceType;
            StartRotation = startRotation;
            EndRotation = endRotation;
            WasSpin = wasSpin;
            WasSpinMini = wasSpinMini;
            WasSpinRaw = wasSpinRaw;
            WasSpinMiniRaw = wasSpinMiniRaw;
        }
    }
}