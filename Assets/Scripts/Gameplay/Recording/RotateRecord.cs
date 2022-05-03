using UnityEngine;

namespace Blockstacker.Gameplay.Recording
{
    public struct RotateRecord : IRecord
    {
        public float Time { get; }
        public RotateDirection Direction { get; }
        public Vector2Int Kick { get; }
        public bool WasLast { get; }

        public RotateRecord(float time, RotateDirection direction, Vector2Int kick, bool wasLast)
        {
            Time = time;
            Direction = direction;
            Kick = kick;
            WasLast = wasLast;
        }
    }
}