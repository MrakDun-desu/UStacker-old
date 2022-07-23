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
        private readonly AllowedSpins _allowedSpins;

        public SpinHandler(RotationSystem rotationSystem, AllowedSpins allowedSpins)
        {
            _rotationSystem = rotationSystem;
            _allowedSpins = allowedSpins;
        }

        public bool TryKick(Piece piece, Board board, RotateDirection direction, out SpinResult result)
        {
            var kickList = GetKickList(piece, direction);

            result = new SpinResult(Vector2Int.zero);

            foreach (var kick in kickList)
            {
                if (!board.CanPlace(piece, kick)) continue;

                result.Kick = kick;
                if (piece.SpinDetectors.Count(spinDetector => !board.IsEmpty(spinDetector.position, kick)) <
                    piece.MinimumSpinDetectors)
                    return true;

                if (piece.FullSpinDetectors.All(spinDetector => !board.IsEmpty(spinDetector.position, kick)) ||
                    _rotationSystem.GetKickTable(piece.Type).FullSpinKicks.Contains(kick))
                    result.WasSpinRaw = true;
                else
                    result.WasSpinMiniRaw = true;

                result = CheckSpinResult(result, piece.Type);

                return true;
            }

            return false;
        }
        
        private SpinResult CheckSpinResult(SpinResult formerResult, string pieceType)
        {
            switch (_allowedSpins)
            {
                case AllowedSpins.Stupid:
                    formerResult.WasSpin = true;
                    formerResult.WasSpinMini = false;
                    return formerResult;
                case AllowedSpins.TSpins:
                    if (pieceType == "t")
                    {
                        formerResult.WasSpin = formerResult.WasSpinRaw;
                        formerResult.WasSpinMini = formerResult.WasSpinMiniRaw;
                    }
                    else
                    {
                        formerResult.WasSpin = false;
                        formerResult.WasSpinMini = false;
                    }
                    return formerResult;
                case AllowedSpins.All:
                    return formerResult;
                case AllowedSpins.None:
                    formerResult.WasSpin = false;
                    formerResult.WasSpinMini = false;
                    return formerResult;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<Vector2Int> GetKickList(Piece piece, RotateDirection direction)
        {
            var kickTable = _rotationSystem.GetKickTable(piece.Type);
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