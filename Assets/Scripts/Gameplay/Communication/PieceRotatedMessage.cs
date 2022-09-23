using Blockstacker.GameSettings.Enums;

namespace Blockstacker.Gameplay.Communication
{
    public record PieceRotatedMessage : MidgameMessage
    {
        public readonly string PieceType;
        public readonly RotationState StartRotation;
        public readonly RotationState EndRotation;
        public readonly bool WasSpin;
        public readonly bool WasSpinMini;
        public readonly bool WasSpinRaw;
        public readonly bool WasSpinMiniRaw;

        public PieceRotatedMessage(string pieceType, RotationState startRotation, RotationState endRotation,
            bool wasSpin, bool wasSpinMini, bool wasSpinRaw, bool wasSpinMiniRaw, double time) : base(time)
        {
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