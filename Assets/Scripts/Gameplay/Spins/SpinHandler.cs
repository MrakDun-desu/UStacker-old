﻿using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Pieces;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UnityEngine;

namespace UStacker.Gameplay.Spins
{
    public class SpinHandler
    {
        private readonly AllowedSpins _allowedSpins;
        private readonly RotationSystem _rotationSystem;

        public SpinHandler(RotationSystem rotationSystem, AllowedSpins allowedSpins)
        {
            _rotationSystem = rotationSystem;
            _allowedSpins = allowedSpins;
        }

        public void RotateWithFirstKick(Piece piece, RotateDirection direction)
        {
            var rotationAngle = direction switch
            {

                RotateDirection.Clockwise => -90,
                RotateDirection.Counterclockwise => 90,
                RotateDirection.OneEighty => 180,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
            piece.Rotate(rotationAngle);

            var kick = GetKickList(piece, direction).First();
            var pieceTransform = piece.transform;
            var piecePosition = pieceTransform.localPosition;
            pieceTransform.localPosition = new Vector3(
                piecePosition.x + kick.x,
                piecePosition.y + kick.y,
                piecePosition.z
            );
        }

        public bool TryKick(Piece piece, Board board, RotateDirection direction, out SpinResult result)
        {
            var kickList = GetKickList(piece, direction);

            result = new SpinResult(Vector2Int.zero);

            foreach (var kick in kickList)
            {
                var actualKick = kick;
                if (piece.Type.StartsWith("giant"))
                    actualKick *= 2;

                if (!board.CanPlace(piece, actualKick)) continue;

                result.Kick = actualKick;
                
                if (_allowedSpins.HasFlag(AllowedSpins.StupidSpinsFlag))
                {
                    result.WasSpin = true;
                    result.WasSpinMini = false;
                }
                
                if (piece.SpinDetectors.Count(spinDetector => !board.IsEmpty(spinDetector.position, actualKick)) <
                    piece.MinimumSpinDetectors)
                    return true;

                if (piece.FullSpinDetectors.All(spinDetector => !board.IsEmpty(spinDetector.position, actualKick)) ||
                    _rotationSystem.GetKickTable(piece.Type).FullSpinKicks.Contains(kick))
                    result.WasSpinRaw = true;
                else
                    result.WasSpinMiniRaw = true;

                CheckSpinResult(result, piece.Type);

                return true;
            }

            return false;
        }

        private void CheckSpinResult(SpinResult formerResult, string pieceType)
        {
            if (_allowedSpins.HasFlag(AllowedSpins.StupidSpinsFlag))
                return;
            
            switch (_allowedSpins)
            {
                case AllowedSpins.All:
                    formerResult.WasSpin = formerResult.WasSpinRaw;
                    formerResult.WasSpinMini = formerResult.WasSpinMiniRaw;
                    return;
                case AllowedSpins.Stupid:
                    formerResult.WasSpin = true;
                    formerResult.WasSpinMini = false;
                    return;
                case AllowedSpins.None:
                    formerResult.WasSpin = false;
                    formerResult.WasSpinMini = false;
                    return;
                case AllowedSpins.ISpins:
                case AllowedSpins.TSpins:
                case AllowedSpins.LSpins:
                case AllowedSpins.JSpins:
                case AllowedSpins.OSpins:
                case AllowedSpins.SSpins:
                case AllowedSpins.ZSpins:
                case AllowedSpins.StupidSpinsFlag:
                default:
                    break;
            }

            if (CheckSpinValidity(pieceType))
            {
                formerResult.WasSpin = formerResult.WasSpinRaw;
                formerResult.WasSpinMini = formerResult.WasSpinMiniRaw;
                return;
            }
            formerResult.WasSpin = false;
            formerResult.WasSpinMini = false;
        }

        private bool CheckSpinValidity(string pieceType)
        {
            if (pieceType.StartsWith("giant"))
                pieceType = pieceType[^1].ToString().ToLowerInvariant();

            return pieceType switch
            {
                "i" => _allowedSpins.HasFlag(AllowedSpins.ISpins),
                "t" => _allowedSpins.HasFlag(AllowedSpins.TSpins),
                "l" => _allowedSpins.HasFlag(AllowedSpins.LSpins),
                "j" => _allowedSpins.HasFlag(AllowedSpins.JSpins),
                "o" => _allowedSpins.HasFlag(AllowedSpins.OSpins),
                "s" => _allowedSpins.HasFlag(AllowedSpins.SSpins),
                "z" => _allowedSpins.HasFlag(AllowedSpins.ZSpins),
                _ => false
            };
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