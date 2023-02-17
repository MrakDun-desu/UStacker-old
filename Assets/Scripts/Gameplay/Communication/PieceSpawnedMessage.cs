using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public readonly struct PieceSpawnedMessage : IMidgameMessage
    {
        public readonly string NextPiece;
        [UsedImplicitly]
        public readonly string SpawnedPiece;
        public double Time { get; }

        public PieceSpawnedMessage(string spawnedPiece, string nextPiece, double time)
        {
            Time = time;
            SpawnedPiece = spawnedPiece;
            NextPiece = nextPiece;
        }
    }
}