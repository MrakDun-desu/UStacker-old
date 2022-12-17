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
        public Vector2Int[] ZeroToThree = new Vector2Int[1];
        public Vector2Int[] ZeroToOne = new Vector2Int[1];
        public Vector2Int[] OneToZero = new Vector2Int[1];
        public Vector2Int[] OneToTwo = new Vector2Int[1];
        public Vector2Int[] TwoToOne = new Vector2Int[1];
        public Vector2Int[] TwoToThree = new Vector2Int[1];
        public Vector2Int[] ThreeToTwo = new Vector2Int[1];
        public Vector2Int[] ThreeToZero = new Vector2Int[1];
        public Vector2Int[] ZeroToTwo = new Vector2Int[1];
        public Vector2Int[] TwoToZero = new Vector2Int[1];
        public Vector2Int[] OneToThree = new Vector2Int[1];
        public Vector2Int[] ThreeToOne = new Vector2Int[1];
    }
}