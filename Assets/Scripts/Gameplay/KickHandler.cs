using System;
using Blockstacker.Gameplay.Enums;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class KickHandler
    {
        private readonly RotationSystem _rotationSystem;

        public KickHandler(RotationSystem rotationSystem)
        {
            _rotationSystem = rotationSystem;
        }

        public bool TryKick(Piece piece, Board board, RotateDirection direction, out Vector2Int result,
            out bool wasLast)
        {
            var table = direction switch
            {
                RotateDirection.Left => TryKickLeft(piece),
                RotateDirection.Right => TryKickRight(piece),
                RotateDirection.OneEighty => TryKickOneEighty(piece),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            result = Vector2Int.zero;
            wasLast = false;
            if (board.CanPlace(piece)) return true;

            for (var i = 0; i < table.Length; i++)
            {
                var kick = table[i];
                if (!board.CanPlace(piece, kick)) continue;
                result = kick;
                if (i == table.Length - 1) wasLast = true;

                return true;
            }

            return false;
        }

        private Vector2Int[] TryKickRight(Piece piece)
        {
            return piece.PieceType switch
            {
                PieceType.IPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.IKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.IKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.IKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.IKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.TPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.TKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.TKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.TKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.TKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.OPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.OKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.OKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.OKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.OKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.LPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.LKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.LKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.LKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.LKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.JPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.JKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.JKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.JKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.JKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.SPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.SKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.SKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.SKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.SKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.ZPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.ZKickTable.ZeroToOne,
                    RotationState.One => _rotationSystem.ZKickTable.OneToTwo,
                    RotationState.Two => _rotationSystem.ZKickTable.TwoToThree,
                    RotationState.Three => _rotationSystem.ZKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2Int[] TryKickLeft(Piece piece)
        {
            return piece.PieceType switch
            {
                PieceType.IPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.IKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.IKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.IKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.IKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.TPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.TKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.TKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.TKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.TKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.OPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.OKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.OKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.OKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.OKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.LPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.LKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.LKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.LKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.LKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.JPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.JKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.JKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.JKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.JKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.SPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.SKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.SKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.SKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.SKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.ZPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.ZKickTable.ZeroToThree,
                    RotationState.One => _rotationSystem.ZKickTable.OneToZero,
                    RotationState.Two => _rotationSystem.ZKickTable.TwoToOne,
                    RotationState.Three => _rotationSystem.ZKickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2Int[] TryKickOneEighty(Piece piece)
        {
            return piece.PieceType switch
            {
                PieceType.IPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.IKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.IKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.IKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.IKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.TPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.TKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.TKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.TKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.TKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.OPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.OKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.OKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.OKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.OKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.LPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.LKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.LKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.LKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.LKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.JPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.JKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.JKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.JKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.JKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.SPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.SKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.SKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.SKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.SKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PieceType.ZPiece => piece.RotationState switch
                {
                    RotationState.Zero => _rotationSystem.ZKickTable.ZeroToTwo,
                    RotationState.One => _rotationSystem.ZKickTable.OneToThree,
                    RotationState.Two => _rotationSystem.ZKickTable.TwoToZero,
                    RotationState.Three => _rotationSystem.ZKickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}