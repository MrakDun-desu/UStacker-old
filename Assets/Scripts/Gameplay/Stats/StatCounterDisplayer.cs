using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GlobalSettings.StatCounting;
using NLua;
using NLua.Exceptions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blockstacker.Gameplay.Stats
{
    public class StatCounterDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private RectTransform _textContainer;
        [SerializeField] private RectTransform _moveHandle;
        [SerializeField] private RectTransform _sizeHandle;

        private static readonly Dictionary<string, Type> RegisterableEvents = new()
        {
            {"CountdownTicked", typeof(CountdownTickedMessage)},
            {"GameEnded", typeof(GameEndedMessage)},
            {"GameLost", typeof(GameLostMessage)},
            {"GamePaused", typeof(GamePausedMessage)},
            {"GameRestarted", typeof(GameRestartedMessage)},
            {"GameResumed", typeof(GameResumedMessage)},
            {"GameStarted", typeof(GameStartedMessage)},
            {"HoldUsed", typeof(HoldUsedMessage)},
            {"InputAction", typeof(InputActionMessage)},
            {"PieceMoved", typeof(PieceMovedMessage)},
            {"PiecePlaced", typeof(PiecePlacedMessage)},
            {"PieceRotated", typeof(PieceRotatedMessage)},
            {"PieceSpawned", typeof(PieceSpawnedMessage)},
        };

        private const string UPDATED_KEY = "CounterUpdated";
        private const string UTILITY_NAME = "StatUtility";
        private const string BOARD_INTERFACE_NAME = "Board";
        private const string STAT_CONTAINER_NAME = "Stats";
        private const string SET_TEXT_NAME = "SetText";

        private Lua _luaState;
        private LuaFunction _updateFunction;

        private MediatorSO _mediator;
        private StatBoardInterface _boardInterface;
        private ReadonlyStatContainer _statContainer;
        private Camera _camera;
        private StatCounterRecord _statCounter;
        private StatUtility _statUtility;

        private Vector2 moveHandlePosition => _moveHandle.position;
        private Vector2 moveHandleSize => _moveHandle.sizeDelta;
        private Vector2 sizeHandlePosition => _sizeHandle.position;
        private Vector2 sizeHandleSize => _sizeHandle.sizeDelta;

        private Vector3 _dragStartPosition;
        private Vector3 _dragStartTransformPosition;
        private bool _isDraggingPosition;
        private bool _isDraggingSize;

        private void RefreshStatCounter()
        {
            _textContainer.localPosition =
                new Vector3(_statCounter.Position.x, _statCounter.Position.y, _textContainer.localPosition.z);
            _textContainer.sizeDelta = _statCounter.Size;

            _luaState = new Lua();
            _luaState.RestrictMaliciousFunctions();
            _luaState[UTILITY_NAME] = _statUtility;
            _luaState[STAT_CONTAINER_NAME] = _statContainer;
            _luaState[BOARD_INTERFACE_NAME] = _boardInterface;
            _luaState.RegisterFunction(SET_TEXT_NAME, this, GetType().GetMethod(nameof(SetText)));
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(_statCounter.Script);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable)
                {
                    events = eventTable;
                }
            }
            catch (LuaException ex)
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Error reading stat counter script!",
                    $"Stat {_statCounter.Name} won't be displayed.\nLua error: {ex.Message}",
                    AlertType.Error
                ));
                gameObject.SetActive(false);
                return;
            }

            if (events is null) return;

            foreach (var entry in RegisterableEvents)
            {
                if (events[entry.Key] is not LuaFunction function) continue;

                void Action(Message message)
                {
                    try
                    {
                        DisplayOutput(function.Call(message));
                    }
                    catch (LuaException ex)
                    {
                        _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                            "Error executing stat counter script!",
                            $"Error executing stat counter script with name {_statCounter.Name}.\nLua error: {ex.Message}",
                            AlertType.Error
                        ));
                        gameObject.SetActive(false);
                    }
                }

                _mediator.Register((Action<Message>) Action, entry.Value);
            }

            if (events[UPDATED_KEY] is not LuaFunction updateFunc) return;
            _updateFunction = updateFunc;

            StartCoroutine(UpdateCor());
        }

        private IEnumerator UpdateCor()
        {
            while (true)
            {
                try
                {
                    DisplayOutput(_updateFunction.Call());
                }
                catch (LuaException ex)
                {
                    _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                        "Error executing user code!",
                        $"Error executing stat counter script with name {_statCounter.Name}.\nLua error: {ex.Message}",
                        AlertType.Error
                    ));
                    gameObject.SetActive(false);
                    
                    yield break;
                }

                yield return new WaitForSeconds(_statCounter.UpdateInterval);
            }
        }

        private void DisplayOutput(object[] output)
        {
            if (output is null) return;
            if (output.Length <= 0 || output[0] is not string outText) return;

            _displayText.text = outText;
        }

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            var mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            HandlePositionDrag(mousePos);
            HandleSizeDrag(mousePos);
        }

        private void HandlePositionDrag(Vector2 mousePos)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame &&
                mousePos.IsInside(moveHandlePosition,
                    new Vector2(moveHandlePosition.x + moveHandleSize.x,
                        moveHandlePosition.y + moveHandleSize.y)))

            {
                _dragStartPosition = mousePos;
                _dragStartTransformPosition = _textContainer.localPosition;
                _isDraggingPosition = true;
            }
            else if (Mouse.current.leftButton.isPressed && _isDraggingPosition)
            {
                var positionDifference = (Vector3) mousePos - _dragStartPosition;
                var newPos = _dragStartTransformPosition + positionDifference;
                newPos.x = Mathf.Round(newPos.x);
                newPos.y = Mathf.Round(newPos.y);
                _textContainer.localPosition = newPos;
                _statCounter.Position = newPos;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isDraggingPosition = false;
            }
        }

        private void HandleSizeDrag(Vector2 mousePos)
        {
            if (mousePos.IsInside(sizeHandlePosition,
                    new Vector2(sizeHandlePosition.x - sizeHandleSize.x, sizeHandlePosition.y - sizeHandleSize.y)) &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isDraggingSize = true;
            }
            else if (Mouse.current.leftButton.isPressed && _isDraggingSize)
            {
                var containerPos = (Vector2) _textContainer.position;
                var sizeDelta = (mousePos - containerPos) / _textContainer.lossyScale;
                sizeDelta.x = Mathf.Round(sizeDelta.x);
                sizeDelta.y = Mathf.Round(sizeDelta.y);
                _textContainer.sizeDelta = sizeDelta;
                _statCounter.Size = sizeDelta;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isDraggingSize = false;
            }
        }

        public void SetText(string text)
        {
            _displayText.text = text;
        }

        public void SetRequiredFields(MediatorSO mediator, StatBoardInterface board,
            ReadonlyStatContainer statContainer, StatUtility statUtility,
            StatCounterRecord statCounter)
        {
            _mediator = mediator;
            _boardInterface = board;
            _statContainer = statContainer;
            _statCounter = statCounter;
            _statUtility = statUtility;
            RefreshStatCounter();
        }
    }
}