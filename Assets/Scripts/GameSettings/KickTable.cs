using System;
using UStacker.GameSettings.Enums;
using UnityEngine;

namespace UStacker.GameSettings
{
    [Serializable]
    public record KickTable
    {
        public RotationState StartState;
        public Vector2Int[] FullSpinKicks = Array.Empty<Vector2Int>();
        public Vector2Int[] ZeroToThree = Array.Empty<Vector2Int>();
        public Vector2Int[] ZeroToOne = Array.Empty<Vector2Int>();
        public Vector2Int[] OneToZero = Array.Empty<Vector2Int>();
        public Vector2Int[] OneToTwo = Array.Empty<Vector2Int>();
        public Vector2Int[] TwoToOne = Array.Empty<Vector2Int>();
        public Vector2Int[] TwoToThree = Array.Empty<Vector2Int>();
        public Vector2Int[] ThreeToTwo = Array.Empty<Vector2Int>();
        public Vector2Int[] ThreeToZero = Array.Empty<Vector2Int>();
        public Vector2Int[] ZeroToTwo = Array.Empty<Vector2Int>();
        public Vector2Int[] TwoToZero = Array.Empty<Vector2Int>();
        public Vector2Int[] OneToThree = Array.Empty<Vector2Int>();
        public Vector2Int[] ThreeToOne = Array.Empty<Vector2Int>();
    }
}