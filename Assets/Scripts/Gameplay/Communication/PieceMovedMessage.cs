namespace Blockstacker.Gameplay.Communication
{
    public record PieceMovedMessage : MidgameMessage
    {
        public int X;
        public int Y;
        public bool WasHardDrop;
        public bool WasSoftDrop;
        public bool HitWall;
    }
}