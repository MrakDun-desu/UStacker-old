using JetBrains.Annotations;
using UStacker.GameSettings.Enums;

namespace UStacker.Gameplay.Communication
{
    public readonly struct PieceRotatedMessage : IMidgameMessage
    {
        [UsedImplicitly]
        public readonly string PieceType;
        [UsedImplicitly]
        public readonly RotationState StartRotation;
        [UsedImplicitly]
        public readonly RotationState EndRotation;
        public readonly bool WasSpin;
        public readonly bool WasSpinMini;
        [UsedImplicitly]
        public readonly bool WasSpinRaw;
        [UsedImplicitly]
        public readonly bool WasSpinMiniRaw;
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