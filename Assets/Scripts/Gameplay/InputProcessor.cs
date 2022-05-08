using Blockstacker.Gameplay.Pieces;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings.Enums;
using Blockstacker.GlobalSettings.Groups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProcessor : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private GameSettingsSO _settings;

        [SerializeField] private Board _board;
        [SerializeField] private PieceSpawner _spawner;
        [SerializeField] private GhostPiece _ghostPiece;
        [SerializeField] private GameTimer _timer;

        [Header("Dependencies filled by initializer")]
        public PieceContainer PieceHolder;

        public KickHandler KickHandler;

        private Piece _activePiece;

        public Piece ActivePiece
        {
            get => _activePiece;
            set
            {
                _pieceIsNull = value is null;
                _activePiece = value;
                if (_pieceIsNull) {
                    _ghostPiece.gameObject.SetActive(false);
                    return;
                }

                _ghostPiece.gameObject.SetActive(true);
                _ghostPiece.SetActivePiece(value);
                _ghostPiece.Render();
            }
        }

        private double _pieceSpawnTime;

        private bool _pieceIsNull = true;

        private HandlingSettings _handling;
        private bool _usedHold;
        private double _normalDropTime;
        private double _effectiveDropTime;
        private double _dropTimer;

        private double _dasRightStart;
        private double _dasLeftStart;
        private double _dasRightTimer;
        private double _dasLeftTimer;
        private double _holdingRightStart = double.PositiveInfinity;
        private double _holdingLeftStart = double.PositiveInfinity;
        private double _holdingRightTimer;
        private double _holdingLeftTimer;

        private double _dasDelay;
        private double _arrTimer = double.PositiveInfinity;
        private bool _dasRightActive;
        private bool _dasLeftActive;

        private void Awake()
        {
            _normalDropTime = 1 / 60d / _settings.Rules.Levelling.Gravity;
            _effectiveDropTime = _normalDropTime;
            _handling = _settings.Rules.Controls.Handling;

            _timer.StartTiming();
            _dropTimer = _normalDropTime;
        }

        public void DeactivateHold()
        {
            PieceHolder.gameObject.SetActive(false);
        }

        #region Input event handling

        public void OnMovePieceLeft(InputAction.CallbackContext ctx)
        {
            var actionTime = _timer.CurrentTime;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            if (ctx.performed) {
                _holdingLeftStart = actionTime;
                _dasLeftStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel) {
                    _dasRightActive = false;
                    _dasRightStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
                MovePiece(Vector2Int.left);
            }
            else if (ctx.canceled) {
                _holdingLeftStart = double.PositiveInfinity;
                _dasLeftStart = double.PositiveInfinity;
                _dasLeftActive = false;
            }
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            var actionTime = _timer.CurrentTime;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeVertical &&
                _effectiveDropTime < _normalDropTime)
                return;

            if (ctx.performed) {
                _holdingRightStart = actionTime;
                _dasRightStart = actionTime;
                if (_handling.AntiDasBehavior != AntiDasBehavior.DontCancel) {
                    _dasLeftActive = false;
                    _dasLeftStart = actionTime;
                }

                if (_pieceIsNull) return;
                if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
                MovePiece(Vector2Int.right);
            }
            else if (ctx.canceled) {
                _holdingRightStart = double.PositiveInfinity;
                _dasRightStart = double.PositiveInfinity;
                _dasRightActive = false;
            }
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            var actionTime = _timer.CurrentTime;
            if (ctx.performed) {
                if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                    (_holdingLeftStart < actionTime ||
                     _holdingRightStart < actionTime)) return;
                _effectiveDropTime = _normalDropTime / _handling.SoftDropFactor;
                _dropTimer = actionTime;
            }

            if (ctx.canceled) {
                _effectiveDropTime = _normalDropTime;
            }
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHardDrop) return;
            if (_handling.DiagonalLockBehavior == DiagonalLockBehavior.PrioritizeHorizontal &&
                (_holdingLeftStart < actionTime||
                 _holdingRightStart < actionTime)) return;

            var movementVector = Vector2Int.down;
            while (_board.CanPlace(ActivePiece, movementVector)) {
                movementVector.y -= 1;
            }

            movementVector.y += 1;
            MovePiece(movementVector, false);
            var linesCleared = _board.Place(ActivePiece);

            var spawnTime = _settings.Rules.Controls.PiecePlacedDelay;
            if (linesCleared)
                spawnTime += _settings.Rules.Controls.LineClearDelay;

            _pieceSpawnTime = actionTime + spawnTime;
            ActivePiece = null;

            _usedHold = false;
            _dasDelay = actionTime + _handling.DasCutDelay;
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx)
        {
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            const int rotationAngle = 90;

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Counterclockwise,
                    out var resultVector,
                    out var wasLast)) {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
        }

        public void OnRotateClockwise(InputAction.CallbackContext ctx)
        {
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            const int rotationAngle = -90;

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Clockwise,
                    out var resultVector,
                    out var wasLast)) {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
        }

        public void OnRotate180(InputAction.CallbackContext ctx)
        {
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.Allow180Spins) return;
            const int rotationAngle = 180;

            ActivePiece.transform.Rotate(Vector3.forward, rotationAngle);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.OneEighty,
                    out var resultVector,
                    out var wasLast)) {
                ActivePiece.transform.Rotate(Vector3.forward, -rotationAngle);
                return;
            }

            MovePiece(resultVector);
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, rotationAngle);
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            var actionTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHold) return;
            if (_usedHold && !_settings.Rules.Controls.UnlimitedHold) return;

            var newPiece = PieceHolder.SwapPiece(ActivePiece);
            if (newPiece == null) {
                _spawner.SpawnPiece();
            }
            else {
                _spawner.SpawnPiece(newPiece);
            }

            _dropTimer = actionTime + _effectiveDropTime;
            _usedHold = true;
        }

        #endregion

        private void Update()
        {
            HandleDas();
            HandleGravity();
            HandlePieceSpawning();
        }

        private void HandleDas()
        {
            var functionStartTime = _timer.CurrentTime;
            if (_holdingLeftStart < functionStartTime && _dasLeftTimer < functionStartTime) {
                _dasLeftTimer = functionStartTime - _dasLeftStart;
                _holdingLeftTimer = functionStartTime - _holdingLeftStart;
            }
            else {
                _dasLeftTimer = 0;
            }

            if (_holdingRightStart < functionStartTime && _dasRightStart < functionStartTime) {
                _dasRightTimer = functionStartTime - _dasRightStart;
                _holdingRightTimer = functionStartTime - _holdingRightStart;
            }
            else {
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
                (_dasLeftTimer < _handling.DelayedAutoShift || dasRightCondition)) {
                _dasRightActive = true;
                _dasLeftActive = false;
                _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
            }

            if (!_dasLeftActive && _dasLeftTimer > _handling.DelayedAutoShift &&
                (_dasRightTimer < _handling.DelayedAutoShift || dasLeftCondition)) {
                _dasLeftActive = true;
                _dasRightActive = false;
                _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
            }

            if (_dasRightActive) {
                while (_arrTimer < functionStartTime) {
                    _arrTimer += _handling.AutomaticRepeatRate;
                    if (_board.CanPlace(ActivePiece, Vector2Int.right))
                        MovePiece(Vector2Int.right);
                    else {
                        _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                        break;
                    }
                }
            }

            if (!_dasLeftActive) return;
            while (_arrTimer < functionStartTime) {
                _arrTimer += _handling.AutomaticRepeatRate;
                if (_board.CanPlace(ActivePiece, Vector2Int.left))
                    MovePiece(Vector2Int.left);
                else {
                    _arrTimer = functionStartTime + _handling.AutomaticRepeatRate;
                    break;
                }
            }
        }

        private void HandleGravity()
        {
            var functionStartTime = _timer.CurrentTime;
            if (_pieceIsNull) return;
            while (_dropTimer < functionStartTime) {
                _dropTimer += _effectiveDropTime;

                if (_board.CanPlace(ActivePiece, Vector2Int.down))
                    MovePiece(Vector2Int.down, false);
                else {
                    _dropTimer = _effectiveDropTime + functionStartTime;
                    break;
                }
            }

            // TODO on touch ground
        }

        private void HandlePieceSpawning()
        {
            var functionStartTime = _timer.CurrentTime;
            if (!_pieceIsNull) return;
            if (_pieceSpawnTime > functionStartTime) return;

            _spawner.SpawnPiece();
            _dropTimer = functionStartTime + _effectiveDropTime;
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

        private static RotationState ChangeRotationState(RotationState state, int value)
        {
            var output = (int)state + value;
            while (output < 0) {
                output += 360;
            }

            output -= output % 90;

            return (RotationState)(output % 360);
        }
    }
}