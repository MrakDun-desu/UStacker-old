using System;
using UnityEngine;

namespace UStacker.Gameplay
{
    [Serializable]
    public record PiecePlacementInfo
    {
        [SerializeField]
        private double _placementTime;
        [SerializeField]
        private int _rotatedAngle;
        [SerializeField]
        private Vector2Int _movement;
        [SerializeField]
        private string _pieceType;

        public double PlacementTime => _placementTime;
        public int RotatedAngle => _rotatedAngle;
        public Vector2Int Movement => _movement;
        public string PieceType => _pieceType;

        public PiecePlacementInfo(double placementTime, int rotatedAngle, Vector2Int movement, string pieceType)
        {
            _placementTime = placementTime;
            _rotatedAngle = rotatedAngle;
            _movement = movement;
            _pieceType = pieceType;
        }
    }
}