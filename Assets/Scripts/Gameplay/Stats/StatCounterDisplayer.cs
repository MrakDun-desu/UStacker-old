using System;
using System.Collections;
using System.Collections.Generic;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GlobalSettings.Music;
using Blockstacker.GlobalSettings.StatCounting;
using JetBrains.Annotations;
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

        private StatCounterRecord _statCounter;

        public StatCounterRecord StatCounter
        {
            get => _statCounter;
            set
            {
                _statCounter = value;
                RefreshStatCounter();
            }
        }

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

        private Lua _luaState;
        private LuaFunction _updateFunction;

        private MediatorSO _mediator;
        private StatBoardInterface _boardInterface;
        private ReadonlyStatContainer _statContainer;
        private Camera _camera;

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
                new Vector3(StatCounter.Position.x, StatCounter.Position.y, _textContainer.localPosition.z);
            _textContainer.sizeDelta = StatCounter.Size;

            _luaState = new Lua();
            _luaState.RestrictMaliciousFunctions();
            _luaState[UTILITY_NAME] = new StatUtility();
            _luaState[STAT_CONTAINER_NAME] = _statContainer;
            _luaState[BOARD_INTERFACE_NAME] = _boardInterface;
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(SoundPackLoader.SoundEffectsScript);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable)
                {
                    events = eventTable;
                }
            }
            catch (LuaException ex)
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Error reading sound effects script!",
                    $"Switching to default sound effects.\nLua error: {ex.Message}",
                    AlertType.Error
                ));
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
                            "Error executing user code!",
                            $"Error executing stat counter script with name {StatCounter.Name}.\nLua error: {ex.Message}",
                            AlertType.Error
                        ));
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
                        $"Error executing stat counter script with name {StatCounter.Name}.\nLua error: {ex.Message}",
                        AlertType.Error
                    ));
                    yield break;
                }

                yield return new WaitForSeconds(StatCounter.UpdateInterval);
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
            // HandlePositionDrag(mousePos);
            HandleSizeDrag(mousePos);
        }

        // private void HandlePositionDrag(Vector2 mousePos)
        // {
        //     if (mousePos.IsInside(moveHandlePosition,
        //             new Vector2(moveHandlePosition.x + moveHandleSize.x, moveHandlePosition.y - moveHandleSize.y)) &&
        //         Mouse.current.leftButton.wasPressedThisFrame)
        //     {
        //         _dragStartPosition = mousePos;
        //         _dragStartTransformPosition = _textContainer.localPosition;
        //         _isDraggingPosition = true;
        //     }
        //     else if (Mouse.current.leftButton.isPressed && _isDraggingPosition)
        //     {
        //         var positionDifference = (Vector3) mousePos - _dragStartPosition;
        //         var newPos = _dragStartTransformPosition + positionDifference;
        //         _textContainer.localPosition = newPos;
        //         StatCounter.Position = newPos;
        //         _isDraggingPosition = false;
        //     }
        // }

        private void HandleSizeDrag(Vector2 mousePos)
        {
            if (mousePos.IsInside(sizeHandlePosition,
                    new Vector2(sizeHandlePosition.x - sizeHandleSize.x, sizeHandlePosition.y + sizeHandleSize.y)) &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                _dragStartPosition = mousePos;
                _isDraggingSize = true;
            }
            else if (Mouse.current.leftButton.isPressed && _isDraggingSize)
            {
                var positionDifference = mousePos - (Vector2) _dragStartPosition;
                var sizeDelta = _textContainer.sizeDelta;
                sizeDelta += positionDifference;
                _textContainer.sizeDelta = sizeDelta;
                _statCounter.Size = sizeDelta;
                _isDraggingSize = false;
            }
        }

        public void SetRequiredFields(MediatorSO mediator, StatBoardInterface board,
            ReadonlyStatContainer statContainer)
        {
            _mediator = mediator;
            _boardInterface = board;
            _statContainer = statContainer;
        }

        private class StatUtility
        {
            [UsedImplicitly]
            public static string FormatTime(double seconds)
            {
                const char paddingChar = '0';
                const int msLength = 3;
                const int sLength = 2;
                const int mLength = 2;

                var timeSpan = TimeSpan.FromSeconds(seconds);

                var ms = timeSpan.Milliseconds.ToString();
                var s = timeSpan.Seconds.ToString();
                var m = timeSpan.Minutes.ToString();
                var h = ((int) timeSpan.TotalHours).ToString();

                ms = ms.PadLeft(msLength, paddingChar);

                if (timeSpan.Minutes == 0 && timeSpan.TotalHours < 1)
                    return $"{s}.{ms}";

                s = s.PadLeft(sLength, paddingChar);

                if (timeSpan.TotalHours < 1)
                    return $"{m}:{s}.{ms}";

                m = m.PadLeft(mLength, paddingChar);

                return $"{h}:{m}:{s}.{ms}";
            }
        }
    }
}