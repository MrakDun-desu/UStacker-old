using System;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Enums;
using Blockstacker.GlobalSettings.Groups;
using Blockstacker.Gameplay.Spins;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    public class InputProcessor : MonoBehaviour
    {
        [Header("Dependencies")] [SerializeField]
        private GameSettingsSO _settings;

        [SerializeField] private Board _board;
        [SerializeField] private PieceSpawner _spawner;
        [SerializeField] private GhostPiece _ghostPiece;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private MediatorSO _mediator;

        [Header("Dependencies filled by initializer")]
        public PieceContainer PieceHolder;

        private Piece _activePiece;
        private double _arrTimer = double.PositiveInfinity;
        private double _dasDelay;
        private bool _dasLeftActive;
        private double _dasLeftStart;
        private double _dasLeftTimer;
        private bool _dasRightActive;

        private double _dasRightStart;
        private double _dasRightTimer;

        private double _dropDisabledUntil;
        private double _dropTimer;
        private double _effectiveDropTime;

        private HandlingSettings _handling;
        private double _hardLockAmount = double.PositiveInfinity;
        private double _holdingLeftStart = double.PositiveInfinity;
        private double _holdingLeftTimer;
        private double _holdingRightStart = double.PositiveInfinity;
        private double _holdingRightTimer;

        private double _lockTime = double.PositiveInfinity;
        private double _normalDropTime;
        private bool _pieceIsNull = true;
        private bool _pieceLocking;

        private double _pieceSpawnTime = double.PositiveInfinity;
        private bool _usedHold;

        private double _currentGravity;

        private bool _lastWasSpin;
        private SpinResult _lastSpinResult;

        public SpinHandler SpinHandler;

        public Piece ActivePiece
        {
            get => _activePiece;
            set
            {
                _usedHold = false;
                _pieceIsNull = value is null;
                _activePiece = value;
                if (_pieceIsNull)
                {
                    _ghostPiece.gameObject.SetActive(false);
                    return;
                }

                _ghostPiece.gameObject.SetActive(true);
                _ghostPiece.SetActivePiece(value);
                _ghostPiece.Render();
            }
        }

        private void Awake()
        {
            _normalDropTime = 1 / 60d / _settings.Rules.Levelling.Gravity;
            _currentGravity = _settings.Rules.Levelling.Gravity;
            _effectiveDropTime = _normalDropTime;
            _dropTimer = _normalDropTime;
            _handling = _settings.Rules.Controls.Handling;
            _dropDisabledUntil = 0;
            _dasDelay = 0;
        }

        public void DeleteActivePiece()
        {
            enabled = false;
            if (_activePiece != null)
                Destroy(_activePiece.gameObject);
            ActivePiece = null;
            var holdPiece = PieceHolder.SwapPiece(null);
            if (holdPiece != null)
                Destroy(holdPiece.gameObject);
        }

        public void ResetProcessor()
        {
            Awake();
            enabled = true;
        }

        private void MovePiece(
            Vector2Int moveVector,
            bool sendMessage,
            double time,
            bool renderGhost = true,
            bool wasHardDrop = false)
        {
            if (moveVector == Vector2Int.zero)
            {
                if (renderGhost)
                    _ghostPiece.Render();
                return;
            }

            if (sendMessage)
            {
                var wasSoftDrop = !wasHardDrop && _effectiveDropTime < _normalDropTime;

                var hitWall = !_board.CanPlace(ActivePiece, Vector2Int.left) ||
                              !_board.CanPlace(ActivePiece, Vector2Int.right);

                _mediator.Send(new PieceMovedMessage
                {
                    Time = time, X = moveVector.x, Y = moveVector.y, WasHardDrop = wasHardDrop,
                    WasSoftDrop = wasSoftDrop, HitWall = hitWall
                });
            }

            _lastWasSpin = false;
            var pieceTransform = ActivePiece.transform;
            var piecePosition = pieceTransform.localPosition;
            piecePosition = new Vector3(
                piecePosition.x + moveVector.x,
                piecePosition.y + moveVector.y,
                piecePosition.z);
            pieceTransform.localPosition = piecePosition;
            if (renderGhost)
                _ghostPiece.Render();
        }

        private SpinResult CheckSpinResult(SpinResult formerResult)
        {
            switch (_settings.Rules.General.AllowedSpins)
            {
                case AllowedSpins.Stupid:
                    formerResult.WasSpin = true;
                    formerResult.WasSpinMini = false;
                    return formerResult;
                case AllowedSpins.TSpins:
                    return ActivePiece.PieceType == "TPiece"
                        ? formerResult
                        : formerResult with {WasSpin = false, WasSpinMini = false};
                case AllowedSpins.All:
                    return formerResult;
                case AllowedSpins.None:
                    return formerResult with {WasSpin = false, WasSpinMini = false};
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePiecePlacement(double placementTime)
        {
            if (_dropDisabledUntil > placementTime) return;
            if (_pieceIsNull) return;

            _dropDisabledUntil = placementTime + _handling.DoubleDropPreventionInterval;
            var movementVector = Vector2Int.down;
            while (_board.CanPlace(ActivePiece, movementVector)) movementVector += Vector2Int.down;

            movementVector -= Vector2Int.down;
            MovePiece(movementVector, true, placementTime, false);

            var linesCleared = _lastWasSpin
                ? _board.Place(ActivePiece, placementTime, _lastSpinResult)
                : _board.Place(ActivePiece, placementTime);

            var spawnTime = _settings.Rules.Controls.PiecePlacedDelay;
            if (linesCleared)
                spawnTime += _settings.Rules.Controls.LineClearDelay;

            _pieceSpawnTime = placementTime + spawnTime;
            ActivePiece = null;
        }

        private void UpdatePiecePlacementVars(double updateTime)
        {
            if (!_pieceLocking) return;
            _lockTime = updateTime + _settings.Rules.Levelling.LockDelay;
            if (_settings.Rules.Controls.OnTouchGround != OnTouchGround.LimitedMoves) return;
            _hardLockAmount -= 1;
            if (_hardLockAmount <= 0) HandlePiecePlacement(updateTime);
        }

        private static RotationState ChangeRotationState(RotationState state, int value)
        {
            var output = (int) state + value;
            while (output < 0) output += 360;

            output -= output % 90;

            return (RotationState) (output % 360);
        }

        #region Input event handling

        public void OnMovePieceLeft(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            Update();
            var actionTime = ctx.time - _timer.EffectiveStartTime;
            var inputMessage = new InputActionMessage {ActionType = ActionType.MoveLeft, Time = actionTime};

            if (ctx.performed)
            {
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});
                _holdingLeftStart = actionTime;
                _dasLeftStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel)
                {
                    _dasRightActive = false;
                    _dasRightStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
                MovePiece(Vector2Int.left, true, actionTime);
                UpdatePiecePlacementVars(actionTime);
            }
            else if (ctx.canceled)
            {
                _holdingLeftStart = double.PositiveInfinity;
                _dasLeftStart = double.PositiveInfinity;
                _dasLeftActive = false;

                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});
            }
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            Update();
            var actionTime = ctx.time - _timer.EffectiveStartTime;
            var inputMessage = new InputActionMessage {ActionType = ActionType.MoveRight, Time = actionTime};

            if (ctx.performed)
            {
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});

                _holdingRightStart = actionTime;
                _dasRightStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel)
                {
                    _dasLeftActive = false;
                    _dasLeftStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
                MovePiece(Vector2Int.right, true, actionTime);
                UpdatePiecePlacementVars(actionTime);
            }
            else if (ctx.canceled)
            {
                _holdingRightStart = double.PositiveInfinity;
                _dasRightStart = double.PositiveInfinity;
                _dasRightActive = false;

                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});
            }
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = ctx.time - _timer.EffectiveStartTime;
            Update();

            var inputMessage = new InputActionMessage
                {ActionType = ActionType.Softdrop, Time = actionTime};

            if (ctx.performed)
            {
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});

                if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                    (_holdingLeftStart < actionTime ||
                     _holdingRightStart < actionTime)) return;

                if (_currentGravity <= 0d)
                {
                    const double normalDropTimeWithZeroGravity = 1d / 60d / .02d;
                    _effectiveDropTime = normalDropTimeWithZeroGravity / _handling.SoftDropFactor;
                }
                else
                {
                    _effectiveDropTime = _normalDropTime / _handling.SoftDropFactor;
                }

                _dropTimer = actionTime;
            }
            else if (ctx.canceled)
            {
                _effectiveDropTime = _normalDropTime;
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});
            }
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            if (_pieceIsNull) return;
            if (!_settings.Rules.Controls.AllowHardDrop) return;

            Update();
            var actionTime = ctx.time - _timer.EffectiveStartTime;
            var inputMessage = new InputActionMessage {ActionType = ActionType.Harddrop, Time = actionTime};
            if (ctx.canceled)
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});

            if (!ctx.performed) return;

            _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            HandlePiecePlacement(actionTime);
        }

        private void HandlePieceRotation(InputAction.CallbackContext ctx, int rotationAngle, RotateDirection direction)
        {
            if (!enabled) return;
            if (_pieceIsNull) return;
            if (direction == RotateDirection.OneEighty && !_settings.Rules.Controls.Allow180Spins) return;

            var actionType = direction switch
            {
                RotateDirection.Clockwise => ActionType.RotateCW,
                RotateDirection.Counterclockwise => ActionType.RotateCCW,
                RotateDirection.OneEighty => ActionType.Rotate180,
                _ => throw new ArgumentOutOfRangeException()
            };
            var actionTime = ctx.time - _timer.EffectiveStartTime;
            var inputMessage = new InputActionMessage {ActionType = actionType, Time = actionTime};
            if (ctx.canceled)
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});
            Update();

            if (!ctx.performed) return;

            _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!SpinHandler.TryKick(
                    ActivePiece,
                    _board,
                    direction,
                    out _lastSpinResult))
            {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            _lastSpinResult = CheckSpinResult(_lastSpinResult);

            MovePiece(_lastSpinResult.Kick, false, actionTime);
            _lastWasSpin = true;

            var previousRotation = ActivePiece.RotationState;
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);

            _mediator.Send(new PieceRotatedMessage
            {
                PieceType = ActivePiece.PieceType, EndRotation = ActivePiece.RotationState,
                StartRotation = previousRotation, Time = actionTime, WasSpin = _lastSpinResult.WasSpin,
                WasSpinMini = _lastSpinResult.WasSpinMini
            });

            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
                _dasDelay = actionTime + _handling.DasCutDelay;

            UpdatePiecePlacementVars(actionTime);
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx) =>
            HandlePieceRotation(ctx, 90, RotateDirection.Counterclockwise);

        public void OnRotateClockwise(InputAction.CallbackContext ctx) =>
            HandlePieceRotation(ctx, -90, RotateDirection.Clockwise);

        public void OnRotate180(InputAction.CallbackContext ctx) =>
            HandlePieceRotation(ctx, 180, RotateDirection.OneEighty);

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            if (_pieceIsNull) return;

            var actionTime = ctx.time - _timer.EffectiveStartTime;
            var inputMessage = new InputActionMessage {ActionType = ActionType.Hold, Time = actionTime};
            if (ctx.canceled)
                _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyUp});
            Update();

            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHold) return;

            _mediator.Send(inputMessage with {KeyActionType = KeyActionType.KeyDown});

            if (_usedHold && !_settings.Rules.Controls.UnlimitedHold) return;

            var newPiece = PieceHolder.SwapPiece(ActivePiece);
            if (newPiece == null)
                _spawner.SpawnPiece();
            else
                _spawner.SpawnPiece(newPiece);

            _dropTimer = actionTime + _effectiveDropTime;
            _lockTime = double.PositiveInfinity;
            _pieceLocking = false;
            _hardLockAmount = double.PositiveInfinity;
            _usedHold = true;
        }

        #endregion

        #region Update

        private void Update()
        {
            HandlePieceSpawning();
            if (_pieceIsNull) return;
            HandleDas();
            HandleGravity();
        }

        private void HandleDas()
        {
            var functionStartTime = _timer.CurrentTime;
            if (_holdingLeftStart < functionStartTime && _dasLeftTimer < functionStartTime)
            {
                _dasLeftTimer = functionStartTime - _dasLeftStart;
                _holdingLeftTimer = functionStartTime - _holdingLeftStart;
            }
            else
            {
                _dasLeftTimer = 0;
            }

            if (_holdingRightStart < functionStartTime && _dasRightStart < functionStartTime)
            {
                _dasRightTimer = functionStartTime - _dasRightStart;
                _holdingRightTimer = functionStartTime - _holdingRightStart;
            }
            else
            {
                _dasRightTimer = 0;
            }

            if (_pieceIsNull) return;
            if (_dasDelay > functionStartTime) return;

            var dasRightCondition = _handling.AntiDasBehavior switch
            {
                AntiDasBehavior.CancelBothDirections => false,
                _ => _holdingRightTimer < _holdingLeftTimer
            };

            var dasLeftCondition = _handling.AntiDasBehavior switch
            {
                AntiDasBehavior.CancelBothDirections => false,
                _ => _holdingRightTimer > _holdingLeftTimer
            };

            if (!_dasRightActive && _dasRightTimer > _handling.DelayedAutoShift &&
                (_dasLeftTimer < _handling.DelayedAutoShift || dasRightCondition))
            {
                _dasRightActive = true;
                _dasLeftActive = false;
                _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
            }

            if (!_dasLeftActive && _dasLeftTimer > _handling.DelayedAutoShift &&
                (_dasRightTimer < _handling.DelayedAutoShift || dasLeftCondition))
            {
                _dasLeftActive = true;
                _dasRightActive = false;
                _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
            }

            if (_dasRightActive)
            {
                var movementVector = Vector2Int.zero;
                while (_arrTimer < functionStartTime)
                {
                    _arrTimer += _handling.AutomaticRepeatRate;
                    if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.right))
                    {
                        movementVector += Vector2Int.right;
                    }
                    else
                    {
                        _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                        break;
                    }
                }

                if (movementVector != Vector2Int.zero)
                    MovePiece(movementVector, true, _arrTimer - _handling.AutomaticRepeatRate);
            }

            if (_dasLeftActive)
            {
                var movementVector = Vector2Int.zero;
                while (_arrTimer < functionStartTime)
                {
                    _arrTimer += _handling.AutomaticRepeatRate;
                    if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.left))
                    {
                        movementVector += Vector2Int.left;
                    }
                    else
                    {
                        _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                        break;
                    }
                }

                if (movementVector != Vector2Int.zero)
                    MovePiece(movementVector, true, _arrTimer - _handling.AutomaticRepeatRate);
            }
        }

        private void HandleGravity()
        {
            var functionStartTime = _timer.CurrentTime;
            if (_pieceIsNull) return;

            var movementVector = Vector2Int.zero;

            while (_dropTimer < functionStartTime)
            {
                _dropTimer += _effectiveDropTime;

                if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.down))
                {
                    movementVector += Vector2Int.down;
                    continue;
                }

                var touchGroundTime = _dropTimer - _effectiveDropTime;

                if (!_pieceLocking)
                {
                    _lockTime = touchGroundTime + _settings.Rules.Levelling.LockDelay;
                    switch (_settings.Rules.Controls.OnTouchGround)
                    {
                        case OnTouchGround.LimitedTime:
                            _hardLockAmount = touchGroundTime + _settings.Rules.Controls.OnTouchGroundAmount;
                            break;
                        case OnTouchGround.LimitedMoves:
                            _hardLockAmount = Math.Floor(_settings.Rules.Controls.OnTouchGroundAmount);
                            break;
                        case OnTouchGround.InfiniteMovement:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _pieceLocking = true;
                }

                _dropTimer = functionStartTime;
                break;
            }

            var lastDropTime = _dropTimer - _effectiveDropTime;

            if (movementVector != Vector2Int.zero)
                MovePiece(movementVector, true, lastDropTime, false);

            if (_lockTime < lastDropTime &&
                _settings.Rules.Controls.OnTouchGround != OnTouchGround.InfiniteMovement)
                HandlePiecePlacement(lastDropTime);

            switch (_settings.Rules.Controls.OnTouchGround)
            {
                case OnTouchGround.LimitedTime:
                    if (_hardLockAmount < lastDropTime)
                        HandlePiecePlacement(lastDropTime);
                    break;
                case OnTouchGround.LimitedMoves:
                case OnTouchGround.InfiniteMovement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePieceSpawning()
        {
            var functionStartTime = _timer.CurrentTime;
            if (!_pieceIsNull) return;
            if (_pieceSpawnTime > functionStartTime) return;

            _lockTime = double.PositiveInfinity;
            _pieceLocking = false;
            _hardLockAmount = double.PositiveInfinity;
            _spawner.SpawnPiece();
            _dropTimer = functionStartTime + _effectiveDropTime;
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Placement))
                _dasDelay = functionStartTime + _handling.DasCutDelay;
        }

        #endregion
    }
}