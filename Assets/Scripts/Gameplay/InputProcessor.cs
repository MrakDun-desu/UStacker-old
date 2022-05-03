using System.Collections;
using Blockstacker.Gameplay.Pieces;
using Blockstacker.Gameplay.Recording;
using Blockstacker.GameSettings;
using Blockstacker.GameSettings.Enums;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Groups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProcessor : MonoBehaviour
    {
        [Header("Dependencies")] [SerializeField]
        private GameSettingsSO _settings;

        [SerializeField] private GameRecorder _recorder;
        [SerializeField] private Board _board;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private PieceSpawner _spawner;

        [Header("Dependencies filled by initializer")]
        public Piece ActivePiece;

        public PieceContainer PieceHolder;
        public KickHandler KickHandler;

        private HandlingSettings _handling;
        private bool _usedHold;
        private float _normalDropTime;
        private float _effectiveDropTime;
        private float _dropTimer;
        
        private float _dasRightTimer;
        private float _dasLeftTimer;
        private bool _holdingRight;
        private bool _holdingLeft;
        private float _dasDelay;

        private Coroutine _dasLeftCor;
        private Coroutine _dasRightCor;

        private void Awake()
        {
            // TODO register messages with mediator
            _normalDropTime = 1 / 60f / _settings.Rules.Levelling.Gravity;
            _effectiveDropTime = _normalDropTime;

            var gameHandling = _settings.Rules.Handling;
            _handling = gameHandling.OverrideHandling switch
            {
                false => AppSettings.Handling,
                true => new HandlingSettings
                {
                    DelayedAutoShift = gameHandling.DelayedAutoShift,
                    AutomaticRepeatRate = gameHandling.AutomaticRepeatRate,
                    SoftDropFactor = gameHandling.SoftDropFactor,
                    DasCutDelay = gameHandling.DasCutDelay,
                    UseDasCancelling = gameHandling.UseDasCancelling,
                    UseDiagonalLock = gameHandling.UseDiagonalLock
                }
            };
        }

        public void DeactivateHold()
        {
            PieceHolder.gameObject.SetActive(false);
        }

        #region Input event handling

        public void OnMovePieceLeft(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _holdingLeft = true;
                if (_handling.UseDasCancelling)
                {
                    StopDasRight();
                    _dasRightTimer = 0;
                }

                if (!_board.CanPlace(ActivePiece, Vector2Int.left)) return;
                MovePiece(Vector2Int.left);
                _recorder.Records.Add(new MoveRecord(_timer.CurrentTime, Vector2Int.left));
            }
            else if (ctx.canceled)
            {
                _holdingLeft = false;
                StopDasLeft();
                _dasLeftTimer = 0;
            }
        }

        public void OnMovePieceRight(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _holdingRight = true;
                if (_handling.UseDasCancelling)
                {
                    StopDasLeft();
                    _dasLeftTimer = 0;
                }

                if (!_board.CanPlace(ActivePiece, Vector2Int.right)) return;
                MovePiece(Vector2Int.right);
                _recorder.Records.Add(new MoveRecord(_timer.CurrentTime, Vector2Int.right));
            }
            else if (ctx.canceled)
            {
                _holdingRight = false;
                StopDasRight();
                _dasRightTimer = 0;
            }
        }

        public void OnSoftDrop(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _effectiveDropTime = _normalDropTime / _handling.SoftDropFactor;
                _dropTimer = 0;
            }
            else if (ctx.canceled)
            {
                _effectiveDropTime = _normalDropTime;
            }
        }

        public void OnHardDrop(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHardDrop) return;
            if (_handling.UseDiagonalLock && (_holdingLeft || _holdingRight)) return;

            var movementVector = Vector2Int.down;
            while (_board.CanPlace(ActivePiece, movementVector))
            {
                movementVector.y -= 1;
            }

            movementVector.y += 1;
            MovePiece(movementVector);
            var linesCleared = _board.Place(ActivePiece);
            _recorder.Records.Add(new MoveRecord(_timer.CurrentTime, movementVector, true));
            _usedHold = false;
            StartCoroutine(PieceSpawnCor(linesCleared));
            _dasDelay = _handling.DasCutDelay;
        }

        public void OnRotateCounterclockwise(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            ActivePiece.transform.Rotate(Vector3.forward, 90);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Counterclockwise,
                    out var resultVector,
                    out var wasLast))
            {
                ActivePiece.transform.Rotate(Vector3.forward, -90);
                return;
            }

            MovePiece(resultVector);
            _recorder.Records.Add(new RotateRecord(_timer.CurrentTime, RotateDirection.Counterclockwise,
                resultVector, wasLast));
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, 90);
            _dasDelay = _handling.DasCutDelay;
        }

        public void OnRotateClockwise(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            ActivePiece.transform.Rotate(Vector3.forward, -90);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Clockwise,
                    out var resultVector,
                    out var wasLast))
            {
                ActivePiece.transform.Rotate(Vector3.forward, 90);
                return;
            }

            MovePiece(resultVector);
            _recorder.Records.Add(new RotateRecord(_timer.CurrentTime, RotateDirection.Clockwise, resultVector,
                wasLast));
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, -90);
            _dasDelay = _handling.DasCutDelay;
        }

        public void OnRotate180(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.Allow180Spins) return;

            ActivePiece.transform.Rotate(Vector3.forward, 180);
            if (!KickHandler.TryKick(
                    ActivePiece,
                    _board,
                    RotateDirection.Clockwise,
                    out var resultVector,
                    out var wasLast))
            {
                ActivePiece.transform.Rotate(Vector3.forward, 180);
                return;
            }

            MovePiece(resultVector);
            _recorder.Records.Add(new RotateRecord(_timer.CurrentTime, RotateDirection.Clockwise, resultVector,
                wasLast));
            ActivePiece.RotationState = ChangeRotationState(ActivePiece.RotationState, 180);
            _dasDelay = _handling.DasCutDelay;
        }

        public void OnSwapHoldPiece(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (!_settings.Rules.Controls.AllowHold) return;
            if (_usedHold && !_settings.Rules.Controls.UnlimitedHold) return;

            var newPiece = PieceHolder.SwapPiece(ActivePiece);
            _recorder.Records.Add(new HoldRecord(_timer.CurrentTime));
            if (newPiece == null)
            {
                _spawner.SpawnPiece();
            }
            else _spawner.SpawnPiece(newPiece);

            _usedHold = true;
        }

        #endregion

        private void Update()
        {
            HandleDas();
            HandleGravity();
        }

        private void HandleDas()
        {
            _dasDelay -= Time.deltaTime;
            if (_dasDelay > 0)
            {
                StopDasLeft();
                StopDasRight();
                return;
            }
            
            if (_holdingLeft) _dasLeftTimer += Time.deltaTime;
            if (_holdingRight) _dasRightTimer += Time.deltaTime;

            if (_dasLeftTimer > _handling.DelayedAutoShift && _dasLeftCor == null && _dasLeftTimer < _dasRightTimer)
            {
                _dasLeftCor = StartCoroutine(DasLeftCoroutine());
                StopDasRight();
            }

            if (_dasRightTimer > _handling.DelayedAutoShift && _dasRightCor == null && _dasRightTimer < _dasLeftTimer)
            {
                _dasRightCor = StartCoroutine(DasRightCoroutine());
                StopDasLeft();
            }
        }

        private void HandleGravity()
        {
            _dropTimer -= Time.deltaTime;
            if (_dropTimer > 0) return;
            
            _dropTimer += _effectiveDropTime;
            if (_board.CanPlace(ActivePiece, Vector2Int.down))
                MovePiece(Vector2Int.down);
            
        }

        private IEnumerator DasLeftCoroutine()
        {
            while (true)
            {
                MovePiece(Vector2Int.left);
                yield return new WaitForSeconds(_handling.AutomaticRepeatRate);
            }
        }

        private IEnumerator DasRightCoroutine()
        {
            while (true)
            {
                MovePiece(Vector2Int.right);
                yield return new WaitForSeconds(_handling.AutomaticRepeatRate);
            }
        }

        private void StopDasLeft()
        {
            if (_dasLeftCor == null) return;
            StopCoroutine(_dasLeftCor);
            _dasLeftCor = null;
        }

        private void StopDasRight()
        {
            if (_dasRightCor == null) return;
            StopCoroutine(_dasRightCor);
            _dasRightCor = null;
        }

        private void MovePiece(Vector2Int moveVector)
        {
            var pieceTransform = ActivePiece.transform;
            var piecePosition = pieceTransform.localPosition;
            piecePosition = new Vector3(
                piecePosition.x + moveVector.x,
                piecePosition.y + moveVector.y,
                piecePosition.z);
            pieceTransform.localPosition = piecePosition;
        }

        private IEnumerator PieceSpawnCor(bool wasClear)
        {
            if (wasClear)
                yield return new WaitForSeconds(_settings.Rules.Controls.LineClearDelay);
            yield return new WaitForSeconds(_settings.Rules.Controls.PiecePlacedDelay);
            _spawner.SpawnPiece();
        }

        private static RotationState ChangeRotationState(RotationState state, int value)
        {
            var output = (int) state + value;
            while (output < 0)
            {
                output += 360;
            }

            output -= output % 90;

            return (RotationState) (output % 360);
        }
    }
}