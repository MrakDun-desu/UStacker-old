using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.Gameplay.Pieces;
using UStacker.Gameplay.Spins;
using UStacker.Gameplay.Timing;
using UStacker.GameSettings;
using UStacker.GameSettings.Enums;
using UStacker.GlobalSettings.Enums;
using UStacker.GlobalSettings.Groups;

namespace UStacker.Gameplay
{
    public class InputProcessor : MonoBehaviour, IGameSettingsDependency
    {
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

        private bool _controlsActive;

        private double _arrTimer = double.PositiveInfinity;
        private bool _bufferedHold;
        private RotateDirection? _bufferedRotation;
        private int _currentActionIndex;
        [SerializeField] private int _currentPieceIndex;
        private double _currentGravity;
        private double _dasDelay;
        private double _dasLeftStart;
        private double _dasLeftTimer;
        private Vector2Int _dasStatus = Vector2Int.zero;
        private double _dasRightStart;
        private double _dasRightTimer;
        private double _dropDisabledUntil;
        private double _dropTime;
        private double _dropTimer;
        private double _hardLockAmount = double.PositiveInfinity;
        private bool _holdingLeft;
        private double _holdingLeftStart = double.PositiveInfinity;
        private double _holdingLeftTimer;
        private bool _holdingRight;
        private double _holdingRightStart = double.PositiveInfinity;
        private double _holdingRightTimer;
        private bool _holdingSoftDrop;
        private bool _softDropActive;
        private double _softDropActivationTime = double.PositiveInfinity;
        private bool _isReplaying;
        private SpinResult _lastSpinResult;
        private int _currentPieceRotation;
        private Vector2Int _currentPieceMovement;
        private bool _lastWasRotation;
        private double _lockDelay;
        private double _lockTime = double.PositiveInfinity;
        private int _lowestPosition;
        private double _normalGravity;
        private bool _pieceIsNull = true;
        private double _pieceSpawnTime = double.PositiveInfinity;
        private bool _usedHold;
        
        private bool DasLeftActive => _dasStatus == Vector2Int.left;
        private bool DasRightActive => _dasStatus == Vector2Int.right;
        private bool _isHardLocking => _hardLockAmount < double.PositiveInfinity;
        private bool _isLocking => _lockTime < double.PositiveInfinity;

        private HandlingSettings _handling;
        private GameSettingsSO.SettingsContainer _settings;
        public SpinHandler SpinHandler { get; set; }

        public List<InputActionMessage> ActionList
        {
            get => _actionList;
            set
            {
                _actionList = value;
                _currentActionIndex = 0;
                _currentPieceIndex = 0;
                _isReplaying = value is not null;
                if (_isReplaying)
                    _controlsActive = false;
            }
        }

        [field: SerializeField] public List<PiecePlacementInfo> PlacementsList { get; set; }

        public Piece ActivePiece
        {
            get => _activePiece;
            set
            {
                _usedHold = false;
                _pieceIsNull = value is null;
                _activePiece = value;
                _currentPieceMovement = new Vector2Int();
                _currentPieceRotation = 0;
                if (_pieceIsNull)
                {
                    _ghostPiece.Disable();
                    return;
                }

                _ghostPiece.Enable();
                _ghostPiece.ActivePiece = value;
                _ghostPiece.Render();
            }
        }

        private void Awake()
        {
            _mediator.Register<GravityChangedMessage>(OnGravityChanged);
            _mediator.Register<LockDelayChangedMessage>(OnLockDelayChanged);
            _timer.BeforeStarted += OnGameStart;
        }

        private void OnDestroy()
        {
            _mediator.Unregister<GravityChangedMessage>(OnGravityChanged);
            _mediator.Unregister<LockDelayChangedMessage>(OnLockDelayChanged);
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
                var rotationAngle = direction switch
                {
                    RotateDirection.Clockwise => -90,
                    RotateDirection.Counterclockwise => 90,
                    RotateDirection.OneEighty => 180,
                    _ => throw new ArgumentOutOfRangeException()
                };
                ChangeActivePieceRotationState(rotationAngle);
                _ghostPiece.Render();
            }
        }

        public void MoveToNextPiece()
        {
            if (_currentPieceIndex >= PlacementsList.Count - 1)
                return;

            if (_currentPieceIndex < PlacementsList.Count - 1)
            {
                if (Math.Abs(_timer.CurrentTime - PlacementsList[_currentPieceIndex].PlacementTime) <
                    double.Epsilon)
                    _currentPieceIndex++;
            }
            
            _timer.SetTime(PlacementsList[_currentPieceIndex].PlacementTime);
        }

        public void MoveToPrevPiece()
        {
            if (_currentPieceIndex <= 0)
            {
                _timer.SetTime(0);
                return;
            }

            _timer.SetTime(PlacementsList[--_currentPieceIndex].PlacementTime);
        }

        private void OnGameStart()
        {
            if (_holdingLeft && !_isReplaying)
                HandleInputAction(new InputActionMessage(ActionType.MoveLeft, KeyActionType.KeyDown, 0d));
            if (_holdingRight && !_isReplaying)
                HandleInputAction(new InputActionMessage(ActionType.MoveRight, KeyActionType.KeyDown, 0d));
        }

        private void CatchUpWithTime(double time)
        {
            CatchUpWithPieces(time);
            CatchUpWithInputs(time);
        }

        private void CatchUpWithInputs(double time)
        {
            while (_currentActionIndex < ActionList.Count)
            {
                var currentAction = ActionList[_currentActionIndex];
                if (currentAction.Time > time) break;
                _currentActionIndex++;
                HandleInputAction(currentAction);
            }
        }

        private void CatchUpWithPieces(double time)
        {
            if (_currentPieceIndex >= PlacementsList.Count - 1) return;
            while (time > PlacementsList[_currentPieceIndex].PlacementTime)
            {
                _currentPieceIndex++;
                if (_currentPieceIndex >= PlacementsList.Count - 1) return;
            }
        }

        private void OnGravityChanged(GravityChangedMessage message)
        {
            _normalGravity = message.Gravity;

            var usedGravity = _normalGravity <= 0 ? _handling.ZeroGravitySoftDropBase : _normalGravity;
            _currentGravity = _softDropActive ? usedGravity * _handling.SoftDropFactor : _normalGravity;

            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = message.Time + _dropTime;
        }

        private void OnLockDelayChanged(LockDelayChangedMessage message)
        {
            _lockDelay = message.LockDelay;
            if (_isLocking)
                _lockTime = message.Time + _lockDelay;
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
            _softDropActive = false;
            _holdingLeft = false;
            _holdingRight = false;
            _holdingLeftStart = double.PositiveInfinity;
            _holdingRightStart = double.PositiveInfinity;
            _softDropActivationTime = double.PositiveInfinity;
            _softDropActive = false;
            _holdingLeftTimer = 0;
            _holdingRightTimer = 0;
            _dasLeftTimer = 0;
            _dasRightTimer = 0;
            _dasLeftStart = 0;
            _dasRightStart = 0;
            _dasStatus = Vector2Int.zero;
            _lowestPosition = int.MaxValue;
            _hardLockAmount = double.PositiveInfinity;
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
            _currentActionIndex = 0;
            _currentPieceIndex = 0;
            _bufferedHold = false;
            _bufferedRotation = null;
            _lastWasRotation = false;
            _pieceSpawnTime = double.PositiveInfinity;
            _usedHold = false;
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
            _currentPieceMovement += moveVector;

            if (sendMessage)
            {
                var wasSoftDrop = !wasHardDrop && _softDropActive;

                var hitWall = moveVector.x != 0 && (!_board.CanPlace(ActivePiece, Vector2Int.left) ||
                                                    !_board.CanPlace(ActivePiece, Vector2Int.right));

                _mediator.Send(new PieceMovedMessage(moveVector.x, moveVector.y,
                    wasHardDrop, wasSoftDrop, hitWall, time));
            }

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

            var lastSpinResult = _lastWasRotation ? _lastSpinResult : null;

            var linesCleared = _board.Place(ActivePiece, placementTime, _currentPieceRotation, _currentPieceMovement,
                lastSpinResult);

            var spawnTime = _settings.Gravity.PiecePlacementDelay;
            if (linesCleared)
                spawnTime += _settings.Gravity.LineClearDelay;

            if (DasLeftActive || DasRightActive)
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

        private void ChangeActivePieceRotationState(int value)
        {
            _currentPieceRotation += value;
            var newState = (int) ActivePiece.RotationState + value;
            while (newState < 0) newState += 360;

            newState -= newState % 90;

            ActivePiece.RotationState = (RotationState) (newState % 360);
        }

        private double ComputeDroptimeFromGravity()
        {
            return 1d / 60d / _currentGravity;
        }

        #region Input event handling

        private void HandleInputAction(InputActionMessage message)
        {
            _mediator.Send(message);
            Update(message.Time, false);
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
                case ActionType.MoveToLeftWall:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        MoveToLeftWallKeyDown(message.Time);
                    break;
                case ActionType.MoveToRightWall:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        MoveToRightWallKeyDown(message.Time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveLeftKeyUp(double actionTime)
        {
            _holdingLeftStart = double.PositiveInfinity;
            _dasLeftStart = double.PositiveInfinity;
            _dasStatus = Vector2Int.zero;
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
                _dasStatus = Vector2Int.zero;
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
            _dasStatus = Vector2Int.zero;
            _dasLeftStart = actionTime;
        }

        private void MoveRightKeyDown(double actionTime)
        {
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical && _holdingSoftDrop)
                return;

            _holdingRightStart = actionTime;
            _dasRightStart = actionTime;
            if (_handling.SimultaneousDasBehavior != SimultaneousDasBehavior.DontCancel)
            {
                _dasStatus = Vector2Int.zero;
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
            _softDropActivationTime = double.PositiveInfinity;
            _softDropActive = false;
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

            _softDropActivationTime = actionTime + _handling.SoftDropDelay;
            HandleGravity(actionTime);
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
            ChangeActivePieceRotationState(rotationAngle);

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
                if (!_handling.CancelDasDelayWithInput)
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

        private void MoveToRightWallKeyDown(double actionTime)
        {
            var movementVector = Vector2Int.right;
            while (_board.CanPlace(ActivePiece, movementVector))
                movementVector += Vector2Int.right;

            movementVector -= Vector2Int.right;
            if (movementVector != Vector2Int.zero)
                MovePiece(movementVector, true, actionTime);
        }

        private void MoveToLeftWallKeyDown(double actionTime)
        {
            var movementVector = Vector2Int.left;
            while (_board.CanPlace(ActivePiece, movementVector))
                movementVector += Vector2Int.left;

            movementVector -= Vector2Int.left;
            if (movementVector != Vector2Int.zero)
                MovePiece(movementVector, true, actionTime);
        }

        private void HandleKeyEvent(InputAction.CallbackContext ctx, ActionType actionType)
        {
            KeyActionType? keyActionType = ctx switch
            {
                {performed: true} => KeyActionType.KeyDown,
                {canceled: true} => KeyActionType.KeyUp,
                _ => null
            };
            if (!_controlsActive && keyActionType == KeyActionType.KeyDown) return;
            if (keyActionType is null) return;

            var actionTime = ctx.time - (Time.realtimeSinceStartupAsDouble - _timer.CurrentTime);
            var actionMessage = new InputActionMessage(
                actionType,
                (KeyActionType) keyActionType,
                actionTime);
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

            HandleKeyEvent(ctx, ActionType.MoveLeft);
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

            HandleKeyEvent(ctx, ActionType.MoveRight);
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.Softdrop);
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowHardDrop)
                HandleKeyEvent(ctx, ActionType.Harddrop);
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.RotateCCW);
        }

        public void OnRotateClockwise(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.RotateCW);
        }

        public void OnRotate180(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.Allow180Spins)
                HandleKeyEvent(ctx, ActionType.Rotate180);
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowHold)
                HandleKeyEvent(ctx, ActionType.Hold);
        }

        public void OnMoveToLeftWall(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowMoveToWall)
                HandleKeyEvent(ctx, ActionType.MoveToLeftWall);
        }

        public void OnMoveToRightWall(InputAction.CallbackContext ctx)
        {
            if (_settings.Controls.AllowMoveToWall)
                HandleKeyEvent(ctx, ActionType.MoveToRightWall);
        }

        #endregion

        #region Update

        public void Update()
        {
            if (_timer.IsRunning)
                Update(_timer.CurrentTime, true);
        }

        public void Update(double time, bool catchUpWithTime)
        {
            if (_isReplaying && catchUpWithTime)
                CatchUpWithTime(time);

            HandleDas(time);
            
            if (!_pieceIsNull)
            {
                HandleGravity(time);
                HandlePieceLockdownAnimation(time);
            }

            HandlePieceSpawning(time);
        }

        private void HandleDas(double time)
        {
            if (_dasDelay > time) return;

            if (_holdingLeftStart < time)
            {
                _dasLeftTimer = time - _dasLeftStart;
                _holdingLeftTimer = time - _holdingLeftStart;
            }
            else
                _dasLeftTimer = 0;

            if (_holdingRightStart < time)
            {
                _dasRightTimer = time - _dasRightStart;
                _holdingRightTimer = time - _holdingRightStart;
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

            if (!DasRightActive && _dasRightTimer > _handling.DelayedAutoShift &&
                (_dasLeftTimer < _handling.DelayedAutoShift || dasRightCondition))
            {
                _dasStatus = Vector2Int.right;
                _arrTimer = _dasRightStart + _handling.DelayedAutoShift;
            }

            if (!DasLeftActive && _dasLeftTimer > _handling.DelayedAutoShift &&
                (_dasRightTimer < _handling.DelayedAutoShift || dasLeftCondition))
            {
                _dasStatus = Vector2Int.left;
                _arrTimer = _dasLeftStart + _handling.DelayedAutoShift;
            }

            if (_arrTimer > time || _dasStatus == Vector2Int.zero) return;

            while (_arrTimer < time)
            {
                if (_board.CanPlace(ActivePiece, _dasStatus))
                {
                    if (_dropTimer < _arrTimer)
                        HandleGravity(_arrTimer);
                    
                    MovePiece(_dasStatus, true, _arrTimer);
                    _arrTimer += _handling.AutomaticRepeatRate;
                }
                else
                {
                    _arrTimer = time;
                    break;
                }
            }
        }

        private void HandleGravity(double time)
        {
            if (_board.CanPlace(ActivePiece, Vector2Int.down) && _isLocking) // there is an empty space below piece
                StopLockdown(false); // stop lockdown even if piece doesn't want to move down

            var movementVector = Vector2Int.zero;
            var lockdownStartedNow = false;

            if (_softDropActivationTime <= _dropTimer)
                ActivateSoftdrop();

            while (_dropTimer <= time) // piece wants to drop
            {
                if (_isLocking) break;

                if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.down))
                {
                    _dropTimer += _dropTime;
                    movementVector += Vector2Int.down; // piece drops one block down
                    if (_softDropActivationTime <= _dropTimer)
                        ActivateSoftdrop();

                    if (_board.CanPlace(ActivePiece, movementVector + Vector2Int.down))
                        continue;

                    // if we got past the if, piece has just touched the ground
                    var lastMovementTime = _dropTimer - _dropTime;

                    // we need to start hard lock when piece touches the ground,
                    // otherwise it would be possible to climb indefinitely
                    StartHardLock(lastMovementTime);

                    if (_settings.Gravity.LockDelayType == LockDelayType.OnTouchGround)
                    {
                        StartLockdown(lastMovementTime);
                        lockdownStartedNow = true;
                        break;
                    }
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

            // if we moved down from the lock start position, we stop, but only if we didn't start this update
            if (_lowestPosition < lowestPosBeforeMovement && !lockdownStartedNow)
                StopLockdown(true);

            if (_lockTime <= time &&
                _settings.Gravity.HardLockType != HardLockType.InfiniteMovement)
                HandlePiecePlacement(_lockTime);
            else if (_settings.Gravity.HardLockType == HardLockType.LimitedTime
                     && _hardLockAmount <= time)
                HandlePiecePlacement(_hardLockAmount);
        }

        private void ActivateSoftdrop()
        {
            if (_softDropActive) return;
            _softDropActive = true;
            var usedGravity = _normalGravity <= 0 ? _handling.ZeroGravitySoftDropBase : _normalGravity;
            _currentGravity = usedGravity * _handling.SoftDropFactor;
            _dropTime = ComputeDroptimeFromGravity();
            _dropTimer = _softDropActivationTime;
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
            _ghostPiece.Disable();
        }

        private void StartHardLock(double lockStart)
        {
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
            if (!_pieceIsNull)
            {
                ActivePiece.Visibility = 1;
                _ghostPiece.Enable();
            }

            if (stopHardlock)
                _hardLockAmount = double.PositiveInfinity;
        }

        private void HandlePieceSpawning(double time)
        {
            if (!_pieceIsNull) return;
            if (_pieceSpawnTime > time)
                return;

            _lockTime = double.PositiveInfinity;
            _hardLockAmount = double.PositiveInfinity;
            _lowestPosition = int.MaxValue;

            if (!_spawner.SpawnPiece(_pieceSpawnTime))
                return;

            _dropTimer = _pieceSpawnTime + _dropTime;
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Placement))
            {
                _arrTimer = _pieceSpawnTime + _handling.DasCutDelay;
                if (!_handling.CancelDasDelayWithInput)
                    _dasDelay = _arrTimer;
            }
            else
                _arrTimer = _pieceSpawnTime;

            _pieceSpawnTime = double.PositiveInfinity;
            // after piece spawn, game needs to update one more time to catch up with gravity
            Update(time, false);
        }

        #endregion
    }
}