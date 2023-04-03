using System;
using System.Collections;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Communication;
using UStacker.GlobalSettings.StatCounting;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NLua;
using NLua.Exceptions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UStacker.Common.LuaApi;
using Logger = UStacker.Common.Logger;
using Random = UStacker.Common.Random;

namespace UStacker.Gameplay.Stats
{
    public class StatCounterDisplayer : MonoBehaviour, IDisposable
    {

        private const string UPDATED_KEY = "CounterUpdated";
        private const string UTILITY_NAME = "StatUtility";
        private const string BOARD_INTERFACE_NAME = "Board";
        private const string STAT_CONTAINER_NAME = "Stats";
        private static StatCounterDisplayer _currentlyUnderMouse;
        [SerializeField] private TMP_Text _displayText;
        [SerializeField] private RectTransform _textContainer;
        [SerializeField] private RectTransform _moveHandle;
        [SerializeField] private RectTransform _sizeHandle;
        private StatBoardInterface _boardInterface;
        private Camera _camera;
        private TweenerCore<Color, Color, ColorOptions> _colorTween;
        private TweenerCore<Color, Color, ColorOptions> _visibilityTween;

        private Vector3 _dragStartPosition;
        private Vector3 _dragStartTransformPosition;
        private bool _isDraggingPosition;
        private bool _isDraggingSize;

        private Lua _luaState;

        private Mediator _mediator;
        private ReadonlyStatContainer _statContainer;
        private StatCounterRecord _statCounter;
        private StatUtility _statUtility;
        private LuaFunction _updateFunction;
        private Random _random;

        private Coroutine _updateRoutine;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
        }

        private void OnDestroy()
        {
            Dispose();
            if (_currentlyUnderMouse == this)
                _currentlyUnderMouse = null;
        }

        private void Update()
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (_currentlyUnderMouse is null)
            {
                if (_textContainer.GetWorldSpaceRect().Contains(mousePos))
                {
                    _currentlyUnderMouse = this;
                    _moveHandle.gameObject.SetActive(true);
                    _sizeHandle.gameObject.SetActive(true);
                }
            }

            if (_currentlyUnderMouse != this) return;

            if (_textContainer.GetWorldSpaceRect().Contains(mousePos) || _isDraggingPosition || _isDraggingSize)
            {
                HandlePositionDrag(mousePos);
                HandleSizeDrag(mousePos);
                return;
            }

            _currentlyUnderMouse = null;
            _moveHandle.gameObject.SetActive(false);
            _sizeHandle.gameObject.SetActive(false);
        }

        private void OnSeedSet(SeedSetMessage message)
        {
            _random.State = message.Seed;
        }

        private void RegisterMethod(string methodName)
        {
            _luaState.RegisterFunction(methodName, this, GetType().GetMethod(methodName));
        }

        private IEnumerator UpdateCor()
        {
            while (true)
            {
                if (!gameObject.activeSelf)
                    yield break;
                
                try
                {
                    DisplayOutput(_updateFunction.Call());
                }
                catch (LuaException ex)
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Error executing stat counter script!",
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

        private void HandlePositionDrag(Vector2 mousePos)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && _moveHandle.GetWorldSpaceRect().Contains(mousePos))
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
            else if (Mouse.current.leftButton.wasReleasedThisFrame) _isDraggingPosition = false;
        }

        private void HandleSizeDrag(Vector2 mousePos)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && _sizeHandle.GetWorldSpaceRect().Contains(mousePos))
                _isDraggingSize = true;
            else if (Mouse.current.leftButton.isPressed && _isDraggingSize)
            {
                var containerPos = (Vector2) _textContainer.position;
                var sizeDelta = (mousePos - containerPos) / _textContainer.lossyScale;
                sizeDelta.x = Mathf.Round(sizeDelta.x);
                sizeDelta.y = Mathf.Round(sizeDelta.y);
                if (sizeDelta.x < 1 || sizeDelta.y < 1) return;
                _textContainer.sizeDelta = sizeDelta;
                _statCounter.Size = sizeDelta;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame) _isDraggingSize = false;
        }
        
        public void RefreshStatCounter(StatCounterRecord statCounter)
        {
            _statCounter = statCounter;
            
            _textContainer.localPosition =
                new Vector3(_statCounter.Position.x, _statCounter.Position.y, _textContainer.localPosition.z);
            _textContainer.sizeDelta = _statCounter.Size;

            Dispose();
            _luaState = CreateLua.WithAllPrerequisites(out _random);
            _luaState[UTILITY_NAME] = _statUtility;
            _luaState[STAT_CONTAINER_NAME] = _statContainer;
            _luaState[BOARD_INTERFACE_NAME] = _boardInterface;
            RegisterMethod(nameof(SetText));
            RegisterMethod(nameof(SetColor));
            RegisterMethod(nameof(SetVisibility));
            RegisterMethod(nameof(AnimateColor));
            RegisterMethod(nameof(AnimateVisibility));
            RegisterMethod(nameof(SetAlignment));
            RegisterMethod(nameof(SetTextSize));
            LuaTable events = null;
            try
            {
                var returnedValue = _luaState.DoString(_statCounter.Script);
                if (returnedValue.Length == 0) return;
                if (returnedValue[0] is LuaTable eventTable) events = eventTable;
            }
            catch (LuaException ex)
            {
                AlertDisplayer.ShowAlert(new Alert(
                    "Error reading stat counter script!",
                    $"Stat {_statCounter.Name} won't be displayed.\nLua error: {ex.Message}",
                    AlertType.Error
                ));
                Logger.Log(_statCounter.Script);
                gameObject.SetActive(false);
                return;
            }

            if (events is null) return;

            foreach (var eventNameObj in events.Keys)
            {
                if (eventNameObj is not string eventName)
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Stat counter {_statCounter.Name} tried registering an invalid event {eventNameObj}",
                        AlertType.Warning));
                    continue;
                }

                if (events[eventNameObj] is not LuaFunction function)
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event handler!",
                        $"Stat counter {_statCounter.Name} tried registering an invalid handler for event {eventName}",
                        AlertType.Warning));
                    continue;
                }
                
                if (eventName == UPDATED_KEY) continue;
                
                if (!RegisterableMessages.Default.ContainsKey(eventName))
                {
                    AlertDisplayer.ShowAlert(new Alert(
                        "Invalid event name!",
                        $"Stat counter {_statCounter.Name} tried registering an invalid event {eventName}",
                        AlertType.Warning));
                    continue;
                }
                
                void Action(IMessage message)
                {
                    if (!gameObject.activeSelf)
                        return;
                    
                    try
                    {
                        DisplayOutput(function.Call(message));
                    }
                    catch (LuaException ex)
                    {
                        AlertDisplayer.ShowAlert(new Alert(
                            "Error executing stat counter script!",
                            $"Error executing stat counter script with name {_statCounter.Name}.\nLua error: {ex.Message}",
                            AlertType.Error
                        ));
                        gameObject.SetActive(false);
                    }
                }

                _mediator.Register((Action<IMessage>) Action, RegisterableMessages.Default[eventName]); 
            }
            
            if (events[UPDATED_KEY] is not LuaFunction updateFunc) return;
            _updateFunction = updateFunc;
        }


        public void Initialize(Mediator mediator, StatBoardInterface board,
            ReadonlyStatContainer statContainer, StatUtility statUtility)
        {
            _mediator = mediator;
            _boardInterface = board;
            _statContainer = statContainer;
            _statUtility = statUtility;
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void OnEnable()
        {
            if (_updateFunction is not null)
                _updateRoutine = StartCoroutine(UpdateCor());
            
            if (_mediator != null)
                _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        private void OnDisable()
        {
            if (_updateRoutine is not null)
            {
                StopCoroutine(_updateRoutine);
                _updateRoutine = null;
            }
            
            _mediator.Register<SeedSetMessage>(OnSeedSet);
        }

        #region Callable functions

        public void SetText(string text)
        {
            _displayText.text = text;
        }

        public void SetVisibility(object value)
        {
            _visibilityTween?.Kill();
            _displayText.alpha = Convert.ToSingle(value);
        }

        public void AnimateVisibility(object alphaObj, object durationObj)
        {
            var alpha = Convert.ToSingle(alphaObj);
            var duration = Convert.ToSingle(durationObj);

            _visibilityTween = DOTween.ToAlpha(() => _displayText.color,
                value => _displayText.color = value,
                alpha,
                duration).SetAutoKill(false);
        }

        public void SetColor(string color)
        {
            _colorTween?.Kill();
            _displayText.color = CreateColor.FromString(color);
        }

        public void AnimateColor(string color, object durationObj)
        {
            var duration = Convert.ToSingle(durationObj);

            _colorTween = DOTween.To(() => _displayText.color,
                value => _displayText.color = value,
                CreateColor.FromString(color),
                duration);
        }

        public void SetAlignment(string alignment)
        {
            _displayText.alignment = alignment.ToLowerInvariant() switch
            {
                "left" => TextAlignmentOptions.Left,
                "right" => TextAlignmentOptions.Right,
                "center" => TextAlignmentOptions.Center,
                "justified" => TextAlignmentOptions.Justified,
                "flush" => TextAlignmentOptions.Flush,
                "top" => TextAlignmentOptions.Top,
                "bottom" => TextAlignmentOptions.Bottom,
                "middle" => TextAlignmentOptions.Midline,
                "baseline" => TextAlignmentOptions.Baseline,
                "capline" => TextAlignmentOptions.Capline,
                _ => _displayText.alignment
            };
        }

        public void SetTextSize(object sizeObj)
        {
            _displayText.enableAutoSizing = false;
            _displayText.fontSize = Convert.ToSingle(sizeObj);
        }

        #endregion

        public void Dispose()
        {
            _luaState?.Dispose();
            _updateFunction?.Dispose();

            _luaState = null;
            _updateFunction = null;
        }
    }
}