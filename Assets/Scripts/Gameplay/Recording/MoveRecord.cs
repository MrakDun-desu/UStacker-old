using UnityEngine;

namespace Blockstacker.Gameplay.Recording
{
    public struct MoveRecord : IRecord
    {
        public float Time { get; }
        public Vector2Int Move { get; }
        public bool WasHarddrop { get; }
        public bool WasSoftdrop { get; }

        public MoveRecord(float time, Vector2Int move, bool wasHarddrop = false, bool wasSoftdrop = false)
        {
            Time = time;
            Move = move;
            WasHarddrop = wasHarddrop;
            WasSoftdrop = wasSoftdrop;
        }
    }
}