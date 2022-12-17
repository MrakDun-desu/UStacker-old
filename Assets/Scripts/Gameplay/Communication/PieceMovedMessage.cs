namespace Blockstacker.Gameplay.Communication
{
    public record PieceMovedMessage : MidgameMessage
    {
        public readonly bool HitWall;
        public readonly bool WasHardDrop;
        public readonly bool WasSoftDrop;
        public readonly int X;
        public readonly int Y;

        public PieceMovedMessage(int x, int y, bool wasHardDrop, bool wasSoftDrop, bool hitWall, double time) : base(time)
        {
            X = x;
            Y = y;
            WasHardDrop = wasHardDrop;
            WasSoftDrop = wasSoftDrop;
            HitWall = hitWall;
        }
    }
}