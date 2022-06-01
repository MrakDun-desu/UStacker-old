using System;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Enums;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Enums;
using Blockstacker.GlobalSettings.Groups;
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

        public KickHandler KickHandler;

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

        private void MovePiece(Vector2Int moveVector, bool renderGhost = true)
        {
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

        private void HandlePiecePlacement(double placementTime)
        {
            if (_dropDisabledUntil > placementTime) return;

            _dropDisabledUntil = placementTime + _handling.DoubleDropPreventionInterval;
            var movementVector = Vector2Int.down;
            while (_board.CanPlace(ActivePiece, movementVector)) movementVector += Vector2Int.down;

            movementVector -= Vector2Int.down;
            MovePiece(movementVector, false);

            _mediator.Send(new LinesDroppedMessage
                {Count = (uint) -movementVector.y, WasHardDrop = true, Time = placementTime});

            var linesCleared = _board.Place(ActivePiece, placementTime);
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
            if (_settings.Rules.Controls.OnTouchGround == OnTouchGround.LimitedMoves)
                _hardLockAmount -= 1;
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
            var actionTime = _timer.CurrentTime;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            if (ctx.performed)
            {
                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.MoveLeft, KeyActionType = KeyActionType.KeyDown, Time = actionTime});
                _holdingLeftStart = actionTime;
                _dasLeftStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel)
                {
                    _dasRightActive = false;
                    _dasRightStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
                MovePiece(Vector2Int.left);
                UpdatePiecePlacementVars(actionTime);
            }
            else if (ctx.canceled)
            {
                _holdingLeftStart = double.PositiveInfinity;
                _dasLeftStart = double.PositiveInfinity;
                _dasLeftActive = false;

                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.MoveLeft, KeyActionType = KeyActionType.KeyUp, Time = actionTime});
            }
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            if (ctx.performed)
            {
                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.MoveRight, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

                _holdingRightStart = actionTime;
                _dasRightStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel)
                {
                    _dasLeftActive = false;
                    _dasLeftStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
                MovePiece(Vector2Int.right);
                UpdatePiecePlacementVars(actionTime);
            }
            else if (ctx.canceled)
            {
                _holdingRightStart = double.PositiveInfinity;
                _dasRightStart = double.PositiveInfinity;
                _dasRightActive = false;

                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.MoveRight, KeyActionType = KeyActionType.KeyUp, Time = actionTime});
            }
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (ctx.performed)
            {
                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.Softdrop, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

                if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                    (_holdingLeftStart < actionTime ||
                     _holdingRightStart < actionTime)) return;
                _effectiveDropTime = _normalDropTime / _handling.SoftDropFactor;
                _dropTimer = actionTime;
            }
            else if (ctx.canceled)
            {
                _effectiveDropTime = _normalDropTime;
                _mediator.Send(new InputActionMessage
                    {ActionType = ActionType.Softdrop, KeyActionType = KeyActionType.KeyUp, Time = actionTime});
            }
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHardDrop) return;

            _mediator.Send(new InputActionMessage
                {ActionType = ActionType.Harddrop, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            HandlePiecePlacement(actionTime);
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            const int rotationAngle = 90;

            _mediator.Send(new InputActionMessage
                {ActionType = ActionType.RotateCCW, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Counterclockwise,
                    out var resultVector))
            {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
                _dasDelay = actionTime + _handling.DasCutDelay;

            UpdatePiecePlacementVars(actionTime);
        }

        public void OnRotateClockwise(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            const int rotationAngle = -90;

            _mediator.Send(new InputActionMessage
                {ActionType = ActionType.RotateCW, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Clockwise,
                    out var resultVector))
            {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
                _dasDelay = actionTime + _handling.DasCutDelay;

            UpdatePiecePlacementVars(actionTime);
        }

        public void OnRotate180(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.Allow180Spins) return;
            const int rotationAngle = 180;

            _mediator.Send(new InputActionMessage
                {ActionType = ActionType.Rotate180, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.OneEighty,
                    out var resultVector))
            {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
                _dasDelay = actionTime + _handling.DasCutDelay;

            UpdatePiecePlacementVars(actionTime);
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (!enabled) return;
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHold) return;

            _mediator.Send(new InputActionMessage
                {ActionType = ActionType.Hold, KeyActionType = KeyActionType.KeyDown, Time = actionTime});

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
                while (_arrTimer < functionStartTime)
                {
                    _arrTimer += _handling.AutomaticRepeatRate;
                    if (_board.CanPlace(ActivePiece, Vector2Int.right))
                    {
                        MovePiece(Vector2Int.right);
                    }
                    else
                    {
                        _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                        break;
                    }
                }

            if (!_dasLeftActive) return;
            while (_arrTimer < functionStartTime)
            {
                _arrTimer += _handling.AutomaticRepeatRate;
                if (_board.CanPlace(ActivePiece, Vector2Int.left))
                {
                    MovePiece(Vector2Int.left);
                }
                else
                {
                    _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                    break;
                }
            }
        }

        private void HandleGravity()
        {
            var functionStartTime = _timer.CurrentTime;
            if (_pieceIsNull) return;

            var movementVector = Vector2Int.down;

            while (_dropTimer < functionStartTime)
            {
                _dropTimer += _effectiveDropTime;

                if (_board.CanPlace(ActivePiece, movementVector))
                {
                    movementVector.y -= 1;
                    continue;
                }

                if (!_pieceLocking)
                {
                    _lockTime = functionStartTime + _settings.Rules.Levelling.LockDelay;
                    switch (_settings.Rules.Controls.OnTouchGround)
                    {
                        case OnTouchGround.LimitedTime:
                            _hardLockAmount = functionStartTime + _settings.Rules.Controls.OnTouchGroundAmount;
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

                _dropTimer = _effectiveDropTime + functionStartTime;
                break;
            }

            movementVector.y += 1;
            MovePiece(movementVector, false);

            _mediator.Send(new LinesDroppedMessage
                {Count = (uint) -movementVector.y, WasHardDrop = false, Time = functionStartTime});

            if (_lockTime < functionStartTime &&
                _settings.Rules.Controls.OnTouchGround != OnTouchGround.InfiniteMovement)
                HandlePiecePlacement(functionStartTime);

            switch (_settings.Rules.Controls.OnTouchGround)
            {
                case OnTouchGround.LimitedTime:
                    if (_hardLockAmount < functionStartTime)
                        HandlePiecePlacement(functionStartTime);
                    break;
                case OnTouchGround.LimitedMoves:
                    if (_hardLockAmount <= 0)
                        HandlePiecePlacement(functionStartTime);
                    break;
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