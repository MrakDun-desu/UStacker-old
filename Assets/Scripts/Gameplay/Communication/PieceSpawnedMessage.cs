namespace Blockstacker.Gameplay.Communication
{
    public record PieceSpawnedMessage : MidgameMessage
    {
        public string SpawnedPiece;
        public string NextPiece;
    }
}