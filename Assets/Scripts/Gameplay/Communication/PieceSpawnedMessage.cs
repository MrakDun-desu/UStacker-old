namespace Blockstacker.Gameplay.Communication
{
    public record PieceSpawnedMessage : MidgameMessage
    {
        public readonly string SpawnedPiece;
        public readonly string NextPiece;

        public PieceSpawnedMessage(string spawnedPiece, string nextPiece, double time) : base(time)
        {
            SpawnedPiece = spawnedPiece;
            NextPiece = nextPiece;
        }
    }
}