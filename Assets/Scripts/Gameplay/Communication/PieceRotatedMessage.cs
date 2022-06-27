using Blockstacker.GameSettings.Enums;

namespace Blockstacker.Gameplay.Communication
{
    public record PieceRotatedMessage : MidgameMessage
    {
        public string PieceType;
        public RotationState StartRotation;
        public RotationState EndRotation;
        public bool WasSpin;
        public bool WasSpinMini;
    }
}