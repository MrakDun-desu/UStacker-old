
/************************************
InputProcessor.cs -- created by Marek Dančo (xdanco00)
*************************************/
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

namespace UStacker.Gameplay.InputProcessing
{
    public class InputProcessor : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Board _board;
        [SerializeField] private Mediator _mediator;
        [SerializeField] private PieceSpawner _spawner;
        [SerializeField] private GhostPiece _ghostPiece;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private PieceContainer _pieceHolder;

        private readonly List<ActionType> _heldActions = new();

        private readonly List<UpdateEvent> _updateEvents = new();

        private List<InputActionMessage> _actionList;
        private Piece _activePiece;
        private UpdateEvent _arrEvent;
        private bool _controlsActive;
        private int _currentActionIndex;
        private double _currentGravity;
        private int _currentPieceIndex;
        private Vector2Int _currentPieceMovement;
        private int _currentPieceRotation;
        private double _dasDelay;
        private UpdateEvent _dasLeftEvent;
        private UpdateEvent _dasRightEvent;
        private Vector2Int _dasStatus = Vector2Int.zero;
        private double _dropDisabledUntil;
        private UpdateEvent _dropEvent;
        private double _dropTime;

        private double _hardLockAmountInternal;
        private UpdateEvent _hardLockdownEvent;
        private double _holdingLeftStart = double.PositiveInfinity;
        private double _holdingRightStart = double.PositiveInfinity;
        private bool _holdingSoftDrop;

        private bool _initialHold;
        private RotateDirection? _initialRotation;
        private bool _isReplaying;
        private SpinResult _lastSpinResult;
        private bool _lastWasRotation;
        private double _lockDelay;
        private UpdateEvent _lockdownEvent;
        private int _lowestPosition;
        private double _normalGravity;
        private bool _pieceIsNull = true;
        private bool _softDropActive;
        private UpdateEvent _softdropEvent;
        private UpdateEvent _spawnEvent;

        private SpinHandler _spinHandler;
        private bool _usedHold;

        private bool _holdingLeft => _holdingLeftStart < double.PositiveInfinity;
        private bool _holdingRight => _holdingRightStart < double.PositiveInfinity;

        private double _hardLockAmount
        {
            get => GameSettings.Gravity.HardLockType switch
            {
                HardLockType.LimitedTime => _hardLockdownEvent.Time,
                HardLockType.LimitedInputs or
                    HardLockType.InfiniteMovement or
                    HardLockType.LimitedMoves => _hardLockAmountInternal,
                _ => throw new ArgumentOutOfRangeException()
            };
            set
            {
                switch (GameSettings.Gravity.HardLockType)
                {
                    case HardLockType.LimitedTime:
                        _hardLockdownEvent.Time = value;
                        break;
                    case HardLockType.LimitedMoves:
                    case HardLockType.LimitedInputs:
                    case HardLockType.InfiniteMovement:
                        _hardLockAmountInternal = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool _isHardLocking => _hardLockAmount < double.PositiveInfinity;
        private bool _isLocking => _lockdownEvent.Time < double.PositiveInfinity;

        private HandlingSettings _handling => OverrideHandling ?? GameSettings.Controls.Handling;

        public double ReplayLength { get; set; }
        public HandlingSettings OverrideHandling { get; set; }

        public List<InputActionMessage> ActionList
        {
            private get => _actionList;
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

        public List<double> PlacementsList { private get; set; }
        public bool ReplayControlsEnabled { get; set; } = true;


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
            _spawnEvent = new UpdateEvent(_updateEvents, EventType.Spawn);
            _dasLeftEvent = new UpdateEvent(_updateEvents, EventType.DasLeft);
            _dasRightEvent = new UpdateEvent(_updateEvents, EventType.DasRight);
            _arrEvent = new UpdateEvent(_updateEvents, EventType.ArrMovement);
            _dropEvent = new UpdateEvent(_updateEvents, EventType.Drop);
            _softdropEvent = new UpdateEvent(_updateEvents, EventType.Softdrop);
            _lockdownEvent = new UpdateEvent(_updateEvents, EventType.Lockdown);
            _hardLockdownEvent = new UpdateEvent(_updateEvents, EventType.HardLockdown);
        }

        private void OnEnable()
        {
            _mediator.Register<GravityChangedMessage>(OnGravityChanged);
            _mediator.Register<LockDelayChangedMessage>(OnLockDelayChanged);
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GravityChangedMessage>(OnGravityChanged);
            _mediator.Unregister<LockDelayChangedMessage>(OnLockDelayChanged);
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChange);
        }

        public GameSettingsSO.SettingsContainer GameSettings { private get; set; }

        public event Action<double> ProcessorUpdated;
        public event Action<InputActionMessage> InputHandled;

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message is {PreviousState: GameState.Unset, NewState: GameState.Initializing})
                Initialize();

            if (message.NewState == GameState.Initializing)
            {
                DeleteActivePiece();
                ResetProcessor();
            }

            if (message is {NewState: GameState.Running, IsReplay: false})
            {
                _controlsActive = true;
                HandlePauseBufferedInputs();
            }

            if (message is {PreviousState: GameState.Running})
                _controlsActive = false;
        }

        private void Initialize()
        {
            _pieceHolder.gameObject.SetActive(GameSettings.Controls.AllowHold);
            if (GameSettings.Controls.AllowHold)
                _pieceHolder.transform.localPosition = new Vector3(
                    -PieceContainer.WIDTH,
                    (int) _board.Height - PieceContainer.HEIGHT
                );

            _spinHandler = new SpinHandler(GameSettings.Controls.ActiveRotationSystem,
                GameSettings.General.AllowedSpins);
        }

        private void HandlePauseBufferedInputs()
        {
            if (_isReplaying)
                return;

            var functionTime = _timer.CurrentTime;

            foreach (var actionType in _heldActions)
                HandleInputAction(new InputActionMessage(actionType, KeyActionType.KeyDown, functionTime));
        }

        public bool TryAutomaticPreSpawnRotation(double time)
        {
            switch (_handling.AutomaticInitialRotation)
            {
                case AutomaticInitialRotation.Disabled:
                    return false;
                case AutomaticInitialRotation.RotateClockwise:
                    RotateKeyDown(time, RotateDirection.Clockwise, true);
                    if (_board.CanPlace(ActivePiece))
                        return true;
                    if (GameSettings.Controls.Allow180Rotations)
                    {
                        RotateKeyDown(time, RotateDirection.OneEighty, true);
                        if (_board.CanPlace(ActivePiece))
                            return true;
                    }

                    RotateKeyDown(time, RotateDirection.Counterclockwise, true);
                    return _board.CanPlace(ActivePiece);
                case AutomaticInitialRotation.RotateCounterclockwise:
                    RotateKeyDown(time, RotateDirection.Counterclockwise, true);
                    if (_board.CanPlace(ActivePiece))
                        return true;
                    if (GameSettings.Controls.Allow180Rotations)
                    {
                        RotateKeyDown(time, RotateDirection.OneEighty, true);
                        if (_board.CanPlace(ActivePiece))
                            return true;
                    }

                    RotateKeyDown(time, RotateDirection.Clockwise, true);
                    return _board.CanPlace(ActivePiece);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddActionToList(InputActionMessage message)
        {
            ActionList.Add(message);
        }

        public void HandleInitialRotation(double time)
        {
            if (!GameSettings.Controls.AllowInitialActions || !_handling.UseInitialRotations ||
                _initialRotation is not { } direction)
                return;

            RotateKeyDown(time, direction, true);
        }

        public void HandleInitialHold(double time, out bool cancelSpawn)
        {
            if (!GameSettings.Controls.AllowInitialActions || !_initialHold || !_handling.UseInitialHold)
            {
                cancelSpawn = false;
                return;
            }

            cancelSpawn = true;
            HoldKeyDown(time, false);
        }

        public void MoveFiveSecondsForward(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveFiveSecondsForward();
        }

        public void MoveFiveSecondsBackward(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveFiveSecondsBackward();
        }

        public void MoveTenthSecondForward(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveTenthSecondForward();
        }

        public void MoveTenthSecondBackward(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveTenthSecondBackward();
        }

        public void MoveToNextPiece(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveToNextPiece();
        }

        public void MoveToPrevPiece(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ReplayControlsEnabled)
                MoveToPrevPiece();
        }

        public void MoveFiveSecondsForward()
        {
            _timer.SetTime(Math.Min(_timer.CurrentTime + 5, ReplayLength));
        }

        public void MoveFiveSecondsBackward()
        {
            _timer.SetTime(Math.Max(0, _timer.CurrentTime - 5));
        }

        public void MoveTenthSecondForward()
        {
            _timer.SetTime(Math.Min(_timer.CurrentTime + 0.1, ReplayLength));
        }

        public void MoveTenthSecondBackward()
        {
            _timer.SetTime(Math.Max(0, _timer.CurrentTime - 0.1));
        }

        public void MoveToNextPiece()
        {
            if (_currentPieceIndex >= PlacementsList.Count - 1)
                return;

            if (_currentPieceIndex < PlacementsList.Count - 1)
                if (Math.Abs(_timer.CurrentTime - PlacementsList[_currentPieceIndex]) < 0.01)
                    _currentPieceIndex++;

            _timer.SetTime(PlacementsList[_currentPieceIndex]);
        }

        public void MoveToPrevPiece()
        {
            if (_currentPieceIndex <= 0)
            {
                _timer.SetTime(0);
                return;
            }

            _timer.SetTime(PlacementsList[--_currentPieceIndex]);
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
            while (time > PlacementsList[_currentPieceIndex])
            {
                _currentPieceIndex++;
                if (_currentPieceIndex >= PlacementsList.Count - 1) return;
            }
        }

        private void OnGravityChanged(GravityChangedMessage message)
        {
            _normalGravity = Math.Max(message.Gravity, 0);

            var usedGravity = _normalGravity <= 0 ? _handling.ZeroGravitySoftDropBase : _normalGravity;
            _currentGravity = _softDropActive ? usedGravity * _handling.SoftDropFactor : _normalGravity;

            _dropTime = ComputeDroptimeFromGravity();
            _dropEvent.Time = message.Time + _dropTime;
        }

        private void OnLockDelayChanged(LockDelayChangedMessage message)
        {
            _lockDelay = Math.Max(message.LockDelay, 0);
            if (_isLocking)
                _lockdownEvent.Time = message.Time + _lockDelay;
        }

        private void DeleteActivePiece()
        {
            if (!_pieceIsNull)
                _activePiece.ReleaseFromPool();
            ActivePiece = null;

            if (!GameSettings.Controls.AllowHold) return;
            var holdPiece = _pieceHolder.SwapPiece(null);
            if (holdPiece == null) return;
            holdPiece.RevertType();
            holdPiece.ReleaseFromPool();
        }

        private void ResetProcessor()
        {
            _holdingSoftDrop = false;
            _softDropActive = false;
            _holdingLeftStart = double.PositiveInfinity;
            _holdingRightStart = double.PositiveInfinity;
            _softDropActive = false;
            _dasStatus = Vector2Int.zero;
            _lowestPosition = int.MaxValue;
            _hardLockAmount = double.PositiveInfinity;
            _normalGravity = GameSettings.Gravity.DefaultGravity;
            _lockDelay = GameSettings.Gravity.DefaultLockDelay;
            _currentGravity = _normalGravity;
            _dropTime = ComputeDroptimeFromGravity();
            _dropDisabledUntil = 0;
            _dasDelay = 0;
            _currentActionIndex = 0;
            _currentPieceIndex = 0;
            _initialHold = false;
            _initialRotation = null;
            _lastWasRotation = false;
            _usedHold = false;

            _dasRightEvent.Time = double.PositiveInfinity;
            _dasLeftEvent.Time = double.PositiveInfinity;
            _softdropEvent.Time = double.PositiveInfinity;
            _arrEvent.Time = double.PositiveInfinity;
            _spawnEvent.Time = double.PositiveInfinity;
            _lockdownEvent.Time = double.PositiveInfinity;
            _hardLockdownEvent.Time = double.PositiveInfinity;
            _dropEvent.Time = _dropTime;
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
            if (GameSettings.Gravity.HardLockType == HardLockType.LimitedMoves)
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

            var updateArr = _dasStatus != Vector2Int.zero && !_board.CanPlace(ActivePiece, _dasStatus);
            var updateDrop =
                double.IsPositiveInfinity(_dropEvent.Time) &&
                !_board.CanPlace(ActivePiece, Vector2Int.down);

            ActivePiece.Move(moveVector);
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
                _dropEvent.Time = time + _dropTime;

            if (updateArr && _board.CanPlace(ActivePiece, _dasStatus))
                _arrEvent.Time = _arrEvent.Time < double.PositiveInfinity ? Math.Max(_arrEvent.Time, time) : time;

            if (updateDrop && _board.CanPlace(ActivePiece, Vector2Int.down))
                _dropEvent.Time = time + _dropTime;

            if (_board.CanPlace(ActivePiece, Vector2Int.down))
                StopLockdown(false);

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

            ActivePiece.SetVisibility(1);
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

            var spawnTime = GameSettings.Gravity.PiecePlacementDelay;
            if (linesCleared)
                spawnTime += GameSettings.Gravity.LineClearDelay;

            if (GameSettings.Controls.AllowHold)
                _pieceHolder.UnmarkUsed();

            _spawnEvent.Time = placementTime + spawnTime;
            ActivePiece = null;
        }

        private void UpdatePiecePlacementVars(double updateTime)
        {
            if (_isHardLocking)
                if (GameSettings.Gravity.HardLockType == HardLockType.LimitedInputs)
                {
                    _hardLockAmount -= 1;
                    if (_hardLockAmount <= 0) HandlePiecePlacement(updateTime);
                }

            if (!_isLocking) return;

            _lockdownEvent.Time = Math.Max(updateTime + _lockDelay, _lockdownEvent.Time);
            _dropEvent.Time = updateTime + _dropTime;
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
            InputHandled?.Invoke(message);
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
                    switch (message.KeyActionType)
                    {
                        case KeyActionType.KeyUp:
                            HoldKeyUp();
                            break;
                        case KeyActionType.KeyDown:
                            HoldKeyDown(message.Time, true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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
                        RotateKeyDown(message.Time, RotateDirection.Clockwise);
                    else
                        RotateKeyUp();
                    break;
                case ActionType.RotateCCW:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        RotateKeyDown(message.Time, RotateDirection.Counterclockwise);
                    else
                        RotateKeyUp();
                    break;
                case ActionType.Rotate180:
                    if (message.KeyActionType == KeyActionType.KeyDown)
                        RotateKeyDown(message.Time, RotateDirection.OneEighty);
                    else
                        RotateKeyUp();
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
            _dasLeftEvent.Time = double.PositiveInfinity;
            _dasStatus = Vector2Int.zero;

            if (!_holdingRight)
                return;

            switch (_handling.SimultaneousDasBehavior)
            {
                case SimultaneousDasBehavior.DontCancel:
                    _dasRightEvent.Time = Math.Max(actionTime, _holdingRightStart + _handling.DelayedAutoShift);
                    break;
                case SimultaneousDasBehavior.CancelFirstDirection:
                case SimultaneousDasBehavior.CancelBothDirections:
                    _dasRightEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveLeftKeyDown(double actionTime)
        {
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical && _holdingSoftDrop)
                return;

            _holdingLeftStart = actionTime;

            _dasStatus = Vector2Int.zero;
            _dasRightEvent.Time = double.PositiveInfinity;
            switch (_handling.SimultaneousDasBehavior)
            {
                case SimultaneousDasBehavior.DontCancel:
                case SimultaneousDasBehavior.CancelFirstDirection:
                    _dasLeftEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                case SimultaneousDasBehavior.CancelBothDirections:
                    if (!_holdingRight)
                        _dasLeftEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_pieceIsNull) return;
            if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
            MovePiece(Vector2Int.left, true, actionTime);
            UpdatePiecePlacementVars(actionTime);
        }

        private void MoveRightKeyUp(double actionTime)
        {
            _holdingRightStart = double.PositiveInfinity;
            _dasRightEvent.Time = double.PositiveInfinity;
            _dasStatus = Vector2Int.zero;

            if (!_holdingLeft)
                return;

            switch (_handling.SimultaneousDasBehavior)
            {
                case SimultaneousDasBehavior.DontCancel:
                    _dasLeftEvent.Time = Math.Max(actionTime, _holdingLeftStart + _handling.DelayedAutoShift);
                    break;
                case SimultaneousDasBehavior.CancelFirstDirection:
                case SimultaneousDasBehavior.CancelBothDirections:
                    _dasLeftEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveRightKeyDown(double actionTime)
        {
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical && _holdingSoftDrop)
                return;

            _holdingRightStart = actionTime;

            _dasStatus = Vector2Int.zero;
            _dasLeftEvent.Time = double.PositiveInfinity;
            switch (_handling.SimultaneousDasBehavior)
            {
                case SimultaneousDasBehavior.DontCancel:
                case SimultaneousDasBehavior.CancelFirstDirection:
                    _dasRightEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                case SimultaneousDasBehavior.CancelBothDirections:
                    if (!_holdingLeft)
                        _dasRightEvent.Time = actionTime + _handling.DelayedAutoShift;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_pieceIsNull) return;
            if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
            MovePiece(Vector2Int.right, true, actionTime);
            UpdatePiecePlacementVars(actionTime);
        }

        private void SoftDropKeyUp(double actionTime)
        {
            _holdingSoftDrop = false;
            _softdropEvent.Time = double.PositiveInfinity;
            _softDropActive = false;
            _currentGravity = _normalGravity;

            _dropTime = ComputeDroptimeFromGravity();
            _dropEvent.Time = actionTime + _dropTime;
        }

        private void SoftDropKeyDown(double actionTime)
        {
            _holdingSoftDrop = true;

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            _softdropEvent.Time = actionTime + _handling.SoftDropDelay;
            _dropEvent.Time = Math.Min(actionTime, _softdropEvent.Time);
        }

        private void HardDropKeyDown(double actionTime)
        {
            if (_dropDisabledUntil > actionTime) return;

            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime ||
                 _holdingRightStart < actionTime)) return;

            HandlePiecePlacement(actionTime, true);
        }

        private void RotateKeyDown(double actionTime, RotateDirection direction, bool firstKickOnly = false)
        {
            if (_handling.InitialActionsType == InitialActionsType.RegisterKeydownDuringSpawn)
                _initialRotation = direction;

            if (_pieceIsNull)
            {
                if (_handling.InitialActionsType == InitialActionsType.RegisterKeypressDuringDelay)
                    _initialRotation = direction;

                return;
            }

            var updateArr = _dasStatus != Vector2Int.zero && !_board.CanPlace(ActivePiece, _dasStatus);
            var updateDrop =
                double.IsPositiveInfinity(_dropEvent.Time) &&
                !_board.CanPlace(ActivePiece, Vector2Int.down);

            var rotationAngle = direction switch
            {
                RotateDirection.Clockwise => -90,
                RotateDirection.Counterclockwise => 90,
                RotateDirection.OneEighty => 180,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            ActivePiece.Rotate(rotationAngle);

            if (!_spinHandler.TryKick(
                    ActivePiece,
                    _board,
                    direction,
                    firstKickOnly,
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

            if (updateArr && _board.CanPlace(ActivePiece, _dasStatus))
                _arrEvent.Time = _arrEvent.Time < double.PositiveInfinity
                    ? Math.Max(_arrEvent.Time, actionTime)
                    : actionTime;

            if (updateDrop && _board.CanPlace(ActivePiece, Vector2Int.down))
                _dropEvent.Time = actionTime + _dropTime;

            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Rotation))
            {
                _arrEvent.Time = Math.Max(actionTime + _handling.DasCutDelay, _arrEvent.Time);
                if (!_handling.CancelDasDelayWithInput)
                    _dasDelay = _arrEvent.Time;
            }

            UpdatePiecePlacementVars(actionTime);
        }

        private void RotateKeyUp()
        {
            if (_handling.InitialActionsType == InitialActionsType.RegisterKeydownDuringSpawn)
                _initialRotation = null;
        }

        private void HoldKeyDown(double actionTime, bool spawnWithInitial)
        {
            if (_pieceIsNull)
            {
                if (_handling.InitialActionsType == InitialActionsType.RegisterKeypressDuringDelay)
                    _initialHold = true;
                return;
            }

            if (_handling.InitialActionsType == InitialActionsType.RegisterKeydownDuringSpawn)
                _initialHold = true;

            if (_usedHold && !GameSettings.Controls.UnlimitedHold)
            {
                _mediator.Send(new HoldUsedMessage(false, actionTime));
                return;
            }

            _mediator.Send(new HoldUsedMessage(true, actionTime));

            var newPiece = _pieceHolder.SwapPiece(ActivePiece);
            if (!GameSettings.Controls.UnlimitedHold)
                _pieceHolder.MarkUsed();

            if (newPiece == null)
                _spawner.SpawnPiece(actionTime, spawnWithInitial);
            else
                _spawner.SpawnPiece(newPiece, actionTime, spawnWithInitial);

            _dropEvent.Time = actionTime + _dropTime;
            if (_dasStatus != Vector2Int.zero && _board.CanPlace(ActivePiece, _dasStatus))
                _arrEvent.Time = _arrEvent.Time < double.PositiveInfinity
                    ? Math.Max(_arrEvent.Time, actionTime)
                    : actionTime;
            _usedHold = true;
            StopLockdown(true);
        }

        private void HoldKeyUp()
        {
            if (_handling.InitialActionsType == InitialActionsType.RegisterKeydownDuringSpawn)
                _initialHold = false;
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
            KeyActionType? keyActionTypeGuess = ctx switch
            {
                {performed: true} => KeyActionType.KeyDown,
                {canceled: true} => KeyActionType.KeyUp,
                _ => null
            };

            if (keyActionTypeGuess is not { } keyActionType) return;

            if (keyActionType == KeyActionType.KeyDown && !_heldActions.Contains(actionType))
                _heldActions.Add(actionType);
            else
                _heldActions.Remove(actionType);

            if (!_controlsActive && keyActionType == KeyActionType.KeyDown) return;

            var actionTime = ctx.time - (Time.realtimeSinceStartupAsDouble - _timer.CurrentTime);
            var actionMessage = new InputActionMessage(
                actionType,
                keyActionType,
                actionTime);
            HandleInputAction(actionMessage);
        }

        public void OnMovePieceLeft(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.MoveLeft);
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.MoveRight);
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            HandleKeyEvent(ctx, ActionType.Softdrop);
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (GameSettings.Controls.AllowHardDrop)
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
            if (GameSettings.Controls.Allow180Rotations)
                HandleKeyEvent(ctx, ActionType.Rotate180);
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (GameSettings.Controls.AllowHold)
                HandleKeyEvent(ctx, ActionType.Hold);
        }

        public void OnMoveToLeftWall(InputAction.CallbackContext ctx)
        {
            if (GameSettings.Controls.AllowMoveToWall)
                HandleKeyEvent(ctx, ActionType.MoveToLeftWall);
        }

        public void OnMoveToRightWall(InputAction.CallbackContext ctx)
        {
            if (GameSettings.Controls.AllowMoveToWall)
                HandleKeyEvent(ctx, ActionType.MoveToRightWall);
        }

        #endregion

        #region Update

        public void Update()
        {
            var functionTime = _timer.CurrentTime;
            HandlePieceLockdownAnimation(functionTime);

            if (!_timer.IsRunning) return;

            Update(functionTime, true);
            ProcessorUpdated?.Invoke(functionTime);
        }

        public void Update(double time, bool catchUpWithTime)
        {
            if (_isReplaying && catchUpWithTime)
                CatchUpWithTime(time);

            while (true)
            {
                _arrEvent.Time = Math.Max(_dasDelay, _arrEvent.Time);
                var currentEvent = _updateEvents[0];
                if (currentEvent.Time > time)
                    break;

                switch (currentEvent.Type)
                {
                    case EventType.ArrMovement:
                        HandleArrMovement();
                        break;
                    case EventType.DasLeft:
                        HandleDasLeft();
                        break;
                    case EventType.DasRight:
                        HandleDasRight();
                        break;
                    case EventType.Drop:
                        HandleDrop();
                        break;
                    case EventType.Softdrop:
                        HandleSoftDrop();
                        break;
                    case EventType.Lockdown:
                        HandleLockdown();
                        break;
                    case EventType.HardLockdown:
                        HandleHardLockdown();
                        break;
                    case EventType.Spawn:
                        HandleSpawn();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void HandleArrMovement()
        {
            if (_pieceIsNull || _dasStatus == Vector2Int.zero || !_board.CanPlace(ActivePiece, _dasStatus))
            {
                _arrEvent.Time = double.PositiveInfinity;
                return;
            }

            MovePiece(_dasStatus, true, _arrEvent.Time);
            _arrEvent.Time += _handling.AutomaticRepeatRate;
        }

        private void HandleDasRight()
        {
            _dasStatus = Vector2Int.right;
            _arrEvent.Time = _dasRightEvent.Time;
            _dasRightEvent.Time = double.PositiveInfinity;
        }

        private void HandleDasLeft()
        {
            _dasStatus = Vector2Int.left;
            _arrEvent.Time = _dasLeftEvent.Time;
            _dasLeftEvent.Time = double.PositiveInfinity;
        }

        private void HandleDrop()
        {
            if (_pieceIsNull)
            {
                _dropEvent.Time = double.PositiveInfinity;
                return;
            }

            if (!_board.CanPlace(ActivePiece, Vector2Int.down))
            {
                StartLockdown(_dropEvent.Time);
                _dropEvent.Time = double.PositiveInfinity;
                return;
            }

            var lowestPosBeforeMovement = _lowestPosition;
            var eventTime = _dropEvent.Time;
            MovePiece(Vector2Int.down, true, _dropEvent.Time, false);
            _dropEvent.Time += _dropTime;

            if (_pieceIsNull)
                return;

            if (_lowestPosition < lowestPosBeforeMovement)
                StopLockdown(true);

            if (_board.CanPlace(ActivePiece, Vector2Int.down)) return;

            // if we got past the if, we just touched ground
            StartHardLock(eventTime);
            if (GameSettings.Gravity.LockDelayType == LockDelayType.OnTouchGround)
                StartLockdown(eventTime);
        }

        private void HandleSoftDrop()
        {
            _softDropActive = true;
            var softdropGravity = _normalGravity <= 0 ? _handling.ZeroGravitySoftDropBase : _normalGravity;

            // try to use normal softdrop gravity
            softdropGravity *= _handling.SoftDropFactor;
            // if softdrop gravity is smaller than min, use min, but never less than normal
            var minSoftdropGravity = Math.Max(_handling.MinSoftDropGravity, _normalGravity);
            // if softdrop gravity is larger than max, use max, but never less than normal
            var maxSoftdropGravity = Math.Max(_handling.MaxSoftDropGravity, _normalGravity);

            _currentGravity = Math.Clamp(softdropGravity, minSoftdropGravity, maxSoftdropGravity);

            _dropTime = ComputeDroptimeFromGravity();
            _dropEvent.Time = _softdropEvent.Time + _dropTime;
            _softdropEvent.Time = double.PositiveInfinity;
        }

        private void HandleLockdown()
        {
            HandlePiecePlacement(_lockdownEvent.Time);
            _lockdownEvent.Time = double.PositiveInfinity;
        }

        private void HandleHardLockdown()
        {
            HandlePiecePlacement(_hardLockdownEvent.Time);
            _hardLockdownEvent.Time = double.PositiveInfinity;
        }

        private void HandleSpawn()
        {
            if (!_pieceIsNull)
            {
                _spawnEvent.Time = double.PositiveInfinity;
                return;
            }

            var spawnTime = _spawnEvent.Time;
            _lockdownEvent.Time = double.PositiveInfinity;
            _hardLockAmount = double.PositiveInfinity;
            _lowestPosition = int.MaxValue;

            if (!_spawner.SpawnPiece(spawnTime))
            {
                _spawnEvent.Time = double.PositiveInfinity;
                return;
            }

            _dropEvent.Time = spawnTime + _dropTime;
            if (_handling.DelayDasOn.HasFlag(DelayDasOn.Placement))
            {
                if (_dasStatus != Vector2Int.zero && _board.CanPlace(ActivePiece, _dasStatus))
                    _arrEvent.Time = _arrEvent.Time < double.PositiveInfinity
                        ? Math.Max(spawnTime + _handling.DasCutDelay, _arrEvent.Time)
                        : spawnTime + _handling.DasCutDelay;

                if (!_handling.CancelDasDelayWithInput)
                    _dasDelay = _arrEvent.Time;
            }
            else if (_dasStatus != Vector2Int.zero && _board.CanPlace(ActivePiece, _dasStatus))
            {
                _arrEvent.Time = _arrEvent.Time < double.PositiveInfinity
                    ? Math.Max(_arrEvent.Time, spawnTime)
                    : spawnTime;
            }

            _spawnEvent.Time = double.PositiveInfinity;
        }

        private void HandlePieceLockdownAnimation(double functionStartTime)
        {
            if (!_isLocking || _pieceIsNull) return;

            var lockProgress = (_lockdownEvent.Time - functionStartTime) / _lockDelay;

            ActivePiece.SetVisibility(Mathf.Lerp(1f, .5f, (float) lockProgress));
        }

        private void StartLockdown(double lockStart)
        {
            if (_isLocking || GameSettings.Gravity.HardLockType == HardLockType.InfiniteMovement) return;

            _lockdownEvent.Time = lockStart + _lockDelay;
            _ghostPiece.Disable();
        }

        private void StartHardLock(double lockStart)
        {
            if (_isHardLocking) return;

            switch (GameSettings.Gravity.HardLockType)
            {
                case HardLockType.LimitedTime:
                    _hardLockAmount = lockStart + GameSettings.Gravity.HardLockAmount;
                    break;
                case HardLockType.LimitedMoves:
                case HardLockType.LimitedInputs:
                    _hardLockAmount = Math.Floor(GameSettings.Gravity.HardLockAmount);
                    break;
                case HardLockType.InfiniteMovement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StopLockdown(bool stopHardlock)
        {
            _lockdownEvent.Time = double.PositiveInfinity;
            if (!_pieceIsNull)
            {
                ActivePiece.SetVisibility(1);
                _ghostPiece.Enable();
            }

            if (stopHardlock)
                _hardLockAmount = double.PositiveInfinity;
        }

        #endregion
    }
}
/************************************
end InputProcessor.cs
*************************************/
