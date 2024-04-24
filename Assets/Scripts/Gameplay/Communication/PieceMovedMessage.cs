
/************************************
PieceMovedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public readonly struct PieceMovedMessage : IMidgameMessage
    {
        [UsedImplicitly] public readonly bool HitWall;
        public readonly bool WasHardDrop;
        public readonly bool WasSoftDrop;
        public readonly int X;
        public readonly int Y;
        public double Time { get; }

        public PieceMovedMessage(int x, int y, bool wasHardDrop, bool wasSoftDrop, bool hitWall, double time)
        {
            Time = time;
            X = x;
            Y = y;
            WasHardDrop = wasHardDrop;
            WasSoftDrop = wasSoftDrop;
            HitWall = hitWall;
        }
    }
}
/************************************
end PieceMovedMessage.cs
*************************************/
