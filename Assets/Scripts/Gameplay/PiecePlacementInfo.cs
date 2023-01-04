using System;
using UnityEngine;

namespace UStacker.Gameplay
{
    [Serializable]
    public record PiecePlacementInfo
    {
        public double PlacementTime;
        public int RotatedAngle;
        public Vector2Int Movement;

        public PiecePlacementInfo(double placementTime, int rotatedAngle, Vector2Int movement)
        {
            PlacementTime = placementTime;
            RotatedAngle = rotatedAngle;
            Movement = movement;
        }
    }
}