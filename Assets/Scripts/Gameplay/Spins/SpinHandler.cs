using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.Gameplay.Spins
{
    public class SpinHandler
    {
        private readonly RotationSystem _rotationSystem;

        public SpinHandler(RotationSystem rotationSystem)
        {
            _rotationSystem = rotationSystem;
        }

        public bool TryKick(Piece piece, Board board, RotateDirection direction, out SpinResult result)
        {
            var kickList = GetKickList(piece, direction);

            result = new SpinResult(Vector2Int.zero);

            foreach (var kick in kickList)
            {
                if (!board.CanPlace(piece, kick)) continue;

                result.Kick = kick;
                if (piece.SpinDetectors.Count(spinDetector => !board.IsEmpty(spinDetector.position)) <
                    piece.MinimumSpinDetectors)
                    return true;

                if (piece.FullSpinDetectors.All(spinDetector => !board.IsEmpty(spinDetector.position)) ||
                    _rotationSystem.GetKickTable(piece.PieceType).FullSpinKicks.Contains(kick))
                    result.WasSpin = true;
                else
                    result.WasSpinMini = true;

                return true;
            }

            return false;
        }

        private IEnumerable<Vector2Int> GetKickList(Piece piece, RotateDirection direction)
        {
            var kickTable = _rotationSystem.GetKickTable(piece.PieceType);
            return direction switch
            {
                RotateDirection.Clockwise => piece.RotationState switch
                {
                    RotationState.Zero => kickTable.ZeroToOne,
                    RotationState.One => kickTable.OneToTwo,
                    RotationState.Two => kickTable.TwoToThree,
                    RotationState.Three => kickTable.ThreeToZero,
                    _ => throw new ArgumentOutOfRangeException()
                },
                RotateDirection.Counterclockwise => piece.RotationState switch
                {
                    RotationState.Zero => kickTable.ZeroToThree,
                    RotationState.One => kickTable.OneToZero,
                    RotationState.Two => kickTable.TwoToOne,
                    RotationState.Three => kickTable.ThreeToTwo,
                    _ => throw new ArgumentOutOfRangeException()
                },
                RotateDirection.OneEighty => piece.RotationState switch
                {
                    RotationState.Zero => kickTable.ZeroToTwo,
                    RotationState.One => kickTable.OneToThree,
                    RotationState.Two => kickTable.TwoToZero,
                    RotationState.Three => kickTable.ThreeToOne,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}