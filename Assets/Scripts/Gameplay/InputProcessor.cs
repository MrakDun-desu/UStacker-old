using System;
using System.Collections.Generic;
using System.Linq;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Spins;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings.Enums;
using UStacker.GlobalSettings.Groups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UStacker.Gameplay
{
    public class InputProcessor : MonoBehaviour, IGameSettingsDependency
    {

        private const double ZERO_GRAVITY_REPLACEMENT = .02d;
        [Header("Dependencies")] [SerializeField]
        private Board _board;

        [SerializeField] private PieceSpawner _spawner;
        [SerializeField] private GhostPiece _ghostPiece;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private MediatorSO _mediator;

        [Header("Dependencies filled by initializer")]
        public PieceContainer PieceHolder;
        private List<InputActionMessage> _actionList;

        private Piece _activePiece;
        private double _arrTimer = double.PositiveInfinity;

        private bool _bufferedHold;
        private RotateDirection? _bufferedRotation;

        private bool _controlsActive;

        private int _currentActionIndex;

        private double _currentGravity;
        private double _dasDelay;
        private bool _dasLeftActive;
        private double _dasLeftStart;
        private double _dasLeftTimer;
        private bool _dasRightActive;

        private double _dasRightStart;
        private double _dasRightTimer;

        private double _dropDisabledUntil;
        private double _dropTime;
        private double _dropTimer;

        private HandlingSettings _handling;
        private double _hardLockAmount = double.PositiveInfinity;

        private bool _holdingLeft;
        private double _holdingLeftStart = double.PositiveInfinity;
        private double _holdingLeftTimer;
        private bool _holdingRight;
        private double _holdingRightStart = double.PositiveInfinity;
        private double _holdingRightTimer;
        private bool _holdingSoftDrop;
        private bool _isReplaying;
        private SpinResult _lastSpinResult;

        private bool _lastWasRotation;
        private double _lockDelay;

        private double _lockTime = double.PositiveInfinity;
        private int _lowestPosition;
        private double _normalGravity;
        private bool _pieceIsNull = true;

        private double _pieceSpawnTime = double.PositiveInfinity;

        private GameSettingsSO.SettingsContainer _settings;
        private bool _usedHold;
        private bool _isHardLocking => _hardLockAmount < double.PositiveInfinity;
        private bool _isLocking => _lockTime < double.PositiveInfinity;

        public SpinHandler SpinHandler { get; set; }

        public List<InputActionMessage> ActionList
        {
            get => _actionList;
            set
            {
                _actionList = value;
                _currentActionIndex = 0;
                _isReplaying = value is not null;
                if (_isReplaying)
                    _controlsActive = false;
            }
        }

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
                _ghostPiece.ActivePiece = value;
                _ghostPiece.Render();
            }
        }

        private void Awake()
        {
            _mediator.Register<GravityChangedMessage>(OnGravityChanged);
            _mediator.Register<LockDelayChangedMessage>(OnLockDelayChanged);
            _timer.TimeSet += RestartAndCatchUpWithTime;
            _timer.BeforeStarted += OnGameStart;
        }

        private void OnDestroy()
        {
            _mediator.Unregister<GravityChangedMessage>(OnGravityChanged);
            _mediator.Unregister<LockDelayChangedMessage>(OnLockDelayChanged);
            _timer.TimeSet -= RestartAndCatchUpWithTime;
            _timer.BeforeStarted -= OnGameStart;
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _settings = value;
        }

        public void DisablePieceControls()
        {
            _controlsActive = false;
        }

        public void EnablePieceControls()
        {
            if (!_isReplaying)
                _controlsActive = true;
        }

        public void ExecuteBufferedActions(double time, out bool cancelSpawn)
        {
            cancelSpawn = false;
            if (_bufferedHold)
            {
                _bufferedHold = false;
                cancelSpawn = true;
                HoldKeyDown(time);
            }

            if (_bufferedRotation is { } direction)
            {
                _bufferedRotation = null;
                SpinHandler.RotateWithFirstKick(ActivePiece, direction);
                _ghostPiece.Render();
            }
        }

        private void OnGameStart()
        {
            if (_holdingLeft)
                HandleInputAction(new InputActionMessage(ActionType.MoveLeft, KeyActionType.KeyDown, 0d));
            if (_holdingRight)
                HandleInputAction(new InputActionMessage(ActionType.MoveRight, KeyActionType.KeyDown, 0d));
        }

        private void RestartAndCatchUpWithTime(double newTime)
        {
            _currentActionIndex = 0;
            _spawner.ResetRandomizer(new GameStartedMessage(_settings.General.ActiveSeed));
            DeleteActivePiece();
            _board.ClearAllBlocks();
            _spawner.EmptyAllContainers();
            CatchUpWithTime(newTime);
        }

        private void CatchUpWithTime(double time)
        {
            while (_currentActionIndex < ActionList.Count)
            {
                var currentAction = ActionList[_currentActionIndex];
                if (!(currentAction.Time < time)) break;
                _currentActionIndex++;
                HandleInputAction(currentAction);
            }
        }

        private void OnGravityChanged(GravityChangedMessage message)
        {
            _normalGravity = message.Gravity;
            if (_normalGravity <= 0)
                _currentGravity = _holdingSoftDrop ? ZERO_GRAVITY_REPLACEMENT * _handling.SoftDropFactor : _normalGravity;
            else
                _currentGravity = _holdingSoftDrop ? _normalGravity * _handling.SoftDropFactor : _normalGravity;

            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = message.Time + _dropTime;
        }

        private void OnLockDelayChanged(LockDelayChangedMessage message)
        {
            _lockDelay = message.LockDelay;
        }

        public void DeleteActivePiece()
        {
            if (!_pieceIsNull)
                _activePiece.ReleaseFromPool();
            ActivePiece = null;

            if (!_settings.Controls.AllowHold) return;
            var holdPiece = PieceHolder.SwapPiece(null);
            if (holdPiece == null) return;
            holdPiece.RevertType();
            holdPiece.ReleaseFromPool();
        }

        public void ResetProcessor()
        {
            _holdingSoftDrop = false;
            _holdingLeft = false;
            _holdingRight = false;
            _lowestPosition = int.MaxValue;
            _handling = _settings.Controls.Handling;
            _normalGravity = _settings.Gravity.DefaultGravity;
            _lockDelay = _settings.Gravity.DefaultLockDelay;
            _currentGravity = _normalGravity;
            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = _dropTime;
            _dropDisabledUntil = 0;
            _dasDelay = 0;
            _lockTime = double.PositiveInfinity;
            _arrTimer = double.PositiveInfinity;
        }

        private void MovePiece(
            Vector2Int moveVector,
            bool sendMessage,
            double time,
            bool renderGhost = true,
            bool wasHardDrop = false,
            bool isRotation = false)
        {
            if (_pieceIsNull) return;
            if (!ActivePiece.BlockPositions.Any()) return;
            if (sendMessage)
            {
                var wasSoftDrop = !wasHardDrop && _holdingSoftDrop;

                var hitWall = moveVector.x != 0 && (!_board.CanPlace(ActivePiece, Vector2Int.left) ||
                                                    !_board.CanPlace(ActivePiece, Vector2Int.right));

                _mediator.Send(new PieceMovedMessage(moveVector.x, moveVector.y,
                    wasHardDrop, wasSoftDrop, hitWall, time));
            }

            if (isRotation)
                _lastWasRotation = true;
            else if (moveVector != Vector2Int.zero)
                _lastWasRotation = false;

            var dropPieceAfterMovement = false;
            if (_settings.Gravity.HardLockType == HardLockType.LimitedMoves)
            {
                if (isRotation)
                    _hardLockAmount -= 1;
                else
                    _hardLockAmount -= Mathf.Abs(moveVector.x);

                if (_hardLockAmount <= 0)
                {
                    dropPieceAfterMovement = true;
                    if (!isRotation)
                    {
                        if (moveVector.x > 0)
                            moveVector.x += (int) _hardLockAmount;
                        else
                            moveVector.x -= (int) _hardLockAmount;
                    }
                }
            }

            var pieceTransform = ActivePiece.transform;
            var piecePosition = pieceTransform.localPosition;
            piecePosition = new Vector3(
                piecePosition.x + moveVector.x,
                piecePosition.y + moveVector.y,
                piecePosition.z);
            pieceTransform.localPosition = piecePosition;

            if (_isLocking && _board.CanPlace(ActivePiece, Vector2Int.down))
                _dropTimer = time + _dropTime;

            var blockPositions = ActivePiece.BlockPositions;
            var blockHeights = blockPositions.Select(pos => _board.WorldSpaceToBoardPosition(pos).y);
            _lowestPosition = Mathf.Min(_lowestPosition, blockHeights.Min());

            if (dropPieceAfterMovement)
                HandlePiecePlacement(time);
            if (renderGhost)
                _ghostPiece.Render();
        }

        private void HandlePiecePlacement(double placementTime, bool wasHarddrop = false)
        {
            if (_pieceIsNull) return;

            ActivePiece.Visibility = 1;
            _dropDisabledUntil = placementTime + _handling.DoubleDropPreventionInterval;

            var movementVector = Vector2Int.down;
            while (_board.CanPlace(ActivePiece, movementVector))
                movementVector += Vector2Int.down;

            movementVector -= Vector2Int.down;
            if (movementVector != Vector2Int.zero)
                MovePiece(movementVector, true, placementTime, false, wasHarddrop);

            var linesCleared = _lastWasRotation
                ? _board.Place(ActivePiece, placementTime, _lastSpinResult)
                : _board.Place(ActivePiece, placementTime);

            var spawnTime = _settings.Gravity.PiecePlacementDelay;
            if (linesCleared)
                spawnTime += _settings.Gravity.LineClearDelay;

            if (_dasLeftActive || _dasRightActive)
                _arrTimer += spawnTime;

            if (_settings.Controls.AllowHold)
                PieceHolder.UnmarkUsed();

            _pieceSpawnTime = placementTime + spawnTime;
            ActivePiece = null;
        }

        private void UpdatePiecePlacementVars(double updateTime)
        {
            if (!_isHardLocking) return;

            if (_settings.Gravity.HardLockType == HardLockType.LimitedInputs)
            {
                _hardLockAmount -= 1;
                if (_hardLockAmount <= 0) HandlePiecePlacement(updateTime);
            }

            if (!_isLocking) return;

            _lockTime = Math.Max(updateTime + _lockDelay, _lockTime);
            _dropTimer = updateTime + _dropTime;
        }

        private static RotationState ChangeRotationState(RotationState state, int value)
        {
            var output = (int) state + value;
            while (output < 0) output += 360;

            output -= output % 90;

            return (RotationState) (output % 360);
        }

        private double ComputeDroptimeFromGravity()
        {
            return 1d / 60d / _currentGravity;
        }

        #region Input event handling

        private void HandleInputAction(InputActionMessage message)
        {
            _mediator.Send(message);
            Update(message.Time);
            switch (message.ActionType)
            {
                case ActionType.MoveLeft:
                    switch (message.KeyActionType)
                    {
                        case KeyActionType.KeyUp:
                            MoveLeftKeyUp(message.Time);
                            break;
                        case KeyActionType.KeyDown:
                            MoveLeftKeyDown(message.Time);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ActionType.MoveRight:
                    switch (message.KeyActionType)
                    {
                        case KeyActionType.KeyUp:
                            MoveRightKeyUp(message.Time);
                            break;
                        case KeyActionType.KeyDown:
                            MoveRightKeyDown(message.Time);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ActionType.Hold:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        HoldKeyDown(message.Time);
                    break;
                case ActionType.Harddrop:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        HardDropKeyDown(message.Time);
                    break;
                case ActionType.Softdrop:
                    switch (message.KeyActionType)
                    {
                        case KeyActionType.KeyUp:
                            SoftDropKeyUp(message.Time);
                            break;
                        case KeyActionType.KeyDown:
                            SoftDropKeyDown(message.Time);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ActionType.RotateCW:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        RotateKeyDown(message.Time, -90, RotateDirection.Clockwise);
                    break;
                case ActionType.RotateCCW:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        RotateKeyDown(message.Time, 90, RotateDirection.Counterclockwise);
                    break;
                case ActionType.Rotate180:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        RotateKeyDown(message.Time, 180, RotateDirection.OneEighty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveLeftKeyUp(double actionTime)
        {
            _holdingLeftStart = double.PositiveInfinity;
            _dasLeftStart = double.PositiveInfinity;
            _dasLeftActive = false;
            _dasRightStart = actionTime;
        }

        private void MoveLeftKeyDown(double actionTime)
        {
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical && _holdingSoftDrop)
                return;

            _holdingLeftStart = actionTime;
            _dasLeftStart = actionTime;
            if (_handling.SimultaneousDasBehavior != SimultaneousDasBehavior.DontCancel)
            {
                _dasRightActive = false;
                _dasRightStart = actionTime;
            }

            if (_pieceIsNull) return;
            if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
            MovePiece(Vector2Int.left, true, actionTime);
            UpdatePiecePlacementVars(actionTime);
        }

        private void MoveRightKeyUp(double actionTime)
        {
            _holdingRightStart = double.PositiveInfinity;
            _dasRightStart = double.PositiveInfinity;
            _dasRightActive = false;
            _dasLeftStart = actionTime;
        }

        private void MoveRightKeyDown(double actionTime)
        {
            _holdingRightStart = actionTime;
            _dasRightStart = actionTime;
            if (_handling.SimultaneousDasBehavior != SimultaneousDasBehavior.DontCancel)
            {
                _dasLeftActive = false;
                _dasLeftStart = actionTime;
            }

            if (_pieceIsNull) return;
            if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
            MovePiece(Vector2Int.right, true, actionTime);
            UpdatePiecePlacementVars(actionTime);
        }

        private void SoftDropKeyUp(double actionTime)
        {
            _holdingSoftDrop = false;
            _currentGravity = _normalGravity;

            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = actionTime + _dropTime;
        }

        private void SoftDropKeyDown(double actionTime)
        {
            _holdingSoftDrop = true;

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            if (_currentGravity <= 0d)
                _currentGravity = ZERO_GRAVITY_REPLACEMENT * _handling.SoftDropFactor;
            else
                _currentGravity = _normalGravity * _handling.SoftDropFactor;

            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = actionTime;
            Update(actionTime);
        }

        private void HardDropKeyDown(double actionTime)
        {
            if (_dropDisabledUntil > actionTime) return;

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            HandlePiecePlacement(actionTime, true);
        }

        private void RotateKeyDown(double actionTime, int rotationAngle, RotateDirection direction)
        {
            if (_pieceIsNull)
            {
                _bufferedRotation = direction;
                return;
            }
            ActivePiece.Rotate(rotationAngle);
            if (!SpinHandler.TryKick(
                    ActivePiece,
                    _board,
                    direction,
                    out _lastSpinResult))
            {
                ActivePiece.Rotate(-rotationAngle);
                return;
            }

            var startRotation = ActivePiece.RotationState;
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);

            _mediator.Send(new PieceRotatedMessage(
                ActivePiece.Type,
                startRotation,
                ActivePiece.RotationState,
                _lastSpinResult.WasSpin,
                _lastSpinResult.WasSpinMini,
                _lastSpinResult.WasSpinRaw,
                _lastSpinResult.WasSpinMiniRaw,
                actionTime));

            MovePiece(_lastSpinResult.Kick, false, actionTime, true, false, true);

            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
            {
                _arrTimer = actionTime + _handling.DasCutDelay;
                if (!_handling.CancelDelayWithMovement)
                    _dasDelay = _arrTimer;
            }

            UpdatePiecePlacementVars(actionTime);
        }

        private void HoldKeyDown(double actionTime)
        {
            if (_pieceIsNull)
            {
                _bufferedHold = true;
                return;
            }
            if (_usedHold && !_settings.Controls.UnlimitedHold)
            {
                _mediator.Send(new HoldUsedMessage(false, actionTime));
                return;
            }

            _mediator.Send(new HoldUsedMessage(true, actionTime));

            var newPiece = PieceHolder.SwapPiece(ActivePiece);
            if (!_settings.Controls.UnlimitedHold)
                PieceHolder.MarkUsed();

            if (newPiece == null)
                _spawner.SpawnPiece(actionTime);
            else
                _spawner.SpawnPiece(newPiece, actionTime);

            _dropTimer = actionTime + _dropTime;
            _usedHold = true;
            StopLockdown(true);
        }

        private void SendActionMessage(InputAction.CallbackContext ctx, ActionType actionType)
        {
            KeyActionType? keyActionType = ctx switch
            {
                {performed: true} => KeyActionType.KeyDown,
                {canceled: true} => KeyActionType.KeyUp,
                _ => null
            };
            if (!_controlsActive && keyActionType == KeyActionType.KeyDown) return;
            if (keyActionType is null) return;
            var actionMessage = new InputActionMessage(
                actionType,
                (KeyActionType) keyActionType,
                ctx.time - _timer.EffectiveStartTime);
            HandleInputAction(actionMessage);
        }

        public void OnMovePieceLeft(InputAction.CallbackContext ctx)
        {
            switch (ctx)
            {
                case {performed: true}:
                    _holdingLeft = true;
                    _holdingRight = false;
                    break;
                case {canceled: true}:
                    _holdingLeft = false;
                    break;
            }
            SendActionMessage(ctx, ActionType.MoveLeft);
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            switch (ctx)
            {
                case {performed: true}:
                    _holdingRight = true;
                    _holdingLeft = false;
                    break;
                case {canceled: true}:
                    _holdingRight = false;
                    break;
            }
            SendActionMessage(ctx, ActionType.MoveRight);
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            SendActionMessage(ctx, ActionType.Softdrop);
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowHardDrop)
                SendActionMessage(ctx, ActionType.Harddrop);
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx)
        {
            SendActionMessage(ctx, ActionType.RotateCCW);
        }

        public void OnRotateClockwise(InputAction.CallbackContext ctx)
        {
            SendActionMessage(ctx, ActionType.RotateCW);
        }

        public void OnRotate180(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.Allow180Spins)
                SendActionMessage(ctx, ActionType.Rotate180);
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowHold)
                SendActionMessage(ctx, ActionType.Hold);
        }

        #endregion

        #region Update

        private void Update()
        {
            Update(_timer.CurrentTime);
        }

        private void Update(double time)
        {
            if (_isReplaying)
                CatchUpWithTime(time);

            if (!_pieceIsNull)
            {
                HandleDas(time);
                HandleGravity(time);
                HandlePieceLockdownAnimation(time);
            }

            HandlePieceSpawning();
        }

        private void HandleDas(double functionStartTime)
        {
            if (_dasDelay > functionStartTime) return;

            if (_holdingLeftStart < functionStartTime)
            {
                _dasLeftTimer = functionStartTime - _dasLeftStart;
                _holdingLeftTimer = functionStartTime - _holdingLeftStart;
            }
            else
                _dasLeftTimer = 0;

            if (_holdingRightStart < functionStartTime)
            {
                _dasRightTimer = functionStartTime - _dasRightStart;
                _holdingRightTimer = functionStartTime - _holdingRightStart;
            }
            else
                _dasRightTimer = 0;

            if (_pieceIsNull) return;

            var dasRightCondition = _handling.SimultaneousDasBehavior switch
            {
                SimultaneousDasBehavior.CancelBothDirections => false,
                _ => _holdingRightTimer < _holdingLeftTimer
            };

            var dasLeftCondition = _handling.SimultaneousDasBehavior switch
            {
                SimultaneousDasBehavior.CancelBothDirections => false,
                _ => _holdingRightTimer > _holdingLeftTimer
            };

            if (!_dasRightActive && _dasRightTimer > _handling.DelayedAutoShift &&
                (_dasLeftTimer < _handling.DelayedAutoShift || dasRightCondition))
            {
                _dasRightActive = true;
                _dasLeftActive = false;
                _arrTimer = _dasRightStart + _handling.DelayedAutoShift;
            }

            if (!_dasLeftActive && _dasLeftTimer > _handling.DelayedAutoShift &&
                (_dasRightTimer < _handling.DelayedAutoShift || dasLeftCondition))
            {
                _dasLeftActive = true;
                _dasRightActive = false;
                _arrTimer = _dasLeftStart + _handling.DelayedAutoShift;
            }

            if (_arrTimer > functionStartTime) return;

            if (_dasRightActive)
            {
                var movementVector = Vector2Int.zero;
                while (_arrTimer < functionStartTime)
                    if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.right))
                    {
                        _arrTimer += _handling.AutomaticRepeatRate;
                        movementVector += Vector2Int.right;
                    }
                    else
                        break;

                if (movementVector != Vector2Int.zero)
                    MovePiece(movementVector, true, _arrTimer - _handling.AutomaticRepeatRate);
            }

            if (_dasLeftActive)
            {
                var movementVector = Vector2Int.zero;
                while (_arrTimer < functionStartTime)
                    if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.left))
                    {
                        _arrTimer += _handling.AutomaticRepeatRate;
                        movementVector += Vector2Int.left;
                    }
                    else
                        break;

                if (movementVector != Vector2Int.zero)
                    MovePiece(movementVector, true, _arrTimer - _handling.AutomaticRepeatRate);
            }
        }

        private void HandleGravity(double functionStartTime)
        {
            if (_board.CanPlace(ActivePiece, Vector2Int.down) && _isLocking) // there is an empty space below piece
                StopLockdown(false); // stop lockdown even if piece doesn't want to move down

            var movementVector = Vector2Int.zero;
            var lockdownStartedNow = false;

            while (_dropTimer <= functionStartTime) // piece wants to drop
            {
                if (_isLocking) break; // if piece is already locking, don't bother

                if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.down))
                {
                    _dropTimer += _dropTime;
                    movementVector += Vector2Int.down; // piece drops one block down
                    if (_settings.Gravity.LockDelayType != LockDelayType.OnTouchGround ||
                        _board.CanPlace(ActivePiece, movementVector + Vector2Int.down))
                        continue; // piece touches ground on this movement

                    StartLockdown(_dropTimer - _dropTime); // start lockdown after this movement
                    lockdownStartedNow = true;
                    break;
                }

                // piece can't drop
                StartLockdown(_dropTimer); // start lockdown after this illegal movement
                lockdownStartedNow = true;
                break;
            }

            var lowestPosBeforeMovement = _lowestPosition;
            if (movementVector != Vector2Int.zero)
                MovePiece(movementVector, true, _dropTimer, false);

            if (!_isLocking && !_isHardLocking) return;

            if (_lowestPosition < lowestPosBeforeMovement && !lockdownStartedNow)
                StopLockdown(true);

            if (_lockTime <= functionStartTime &&
                _settings.Gravity.HardLockType != HardLockType.InfiniteMovement)
                HandlePiecePlacement(_lockTime);
            else if (_settings.Gravity.HardLockType == HardLockType.LimitedTime
                     && _hardLockAmount <= functionStartTime)
                HandlePiecePlacement(_hardLockAmount);
        }

        private void HandlePieceLockdownAnimation(double functionStartTime)
        {
            if (!_isLocking || _pieceIsNull) return;

            var lockProgress = (_lockTime - functionStartTime) / _lockDelay;

            ActivePiece.Visibility = Mathf.Lerp(1f, .5f, (float) lockProgress);
        }

        private void StartLockdown(double lockStart)
        {
            if (_isLocking) return;

            _lockTime = lockStart + _lockDelay;

            if (_isHardLocking) return;

            switch (_settings.Gravity.HardLockType)
            {
                case HardLockType.LimitedTime:
                    _hardLockAmount = lockStart + _settings.Gravity.HardLockAmount;
                    break;
                case HardLockType.LimitedMoves:
                case HardLockType.LimitedInputs:
                    _hardLockAmount = Math.Floor(_settings.Gravity.HardLockAmount);
                    break;
                case HardLockType.InfiniteMovement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StopLockdown(bool stopHardlock)
        {
            _lockTime = double.PositiveInfinity;
            if (stopHardlock)
                _hardLockAmount = double.PositiveInfinity;
        }

        private void HandlePieceSpawning()
        {
            if (!_pieceIsNull) return;
            var functionStartTime = _timer.CurrentTime;
            if (_pieceSpawnTime > functionStartTime) return;

            _lockTime = double.PositiveInfinity;
            _hardLockAmount = double.PositiveInfinity;
            _lowestPosition = int.MaxValue;

            if (!_spawner.SpawnPiece(_pieceSpawnTime))
                return;

            _dropTimer = _pieceSpawnTime + _dropTime;
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Placement))
            {
                _arrTimer = _pieceSpawnTime + _handling.DasCutDelay;
                if (!_handling.CancelDelayWithMovement)
                    _dasDelay = _arrTimer;
            }
            else
                _arrTimer = _pieceSpawnTime;

            // after piece spawn, game needs to update one more time to catch up with gravity
            Update();
        }

        #endregion
    }
}