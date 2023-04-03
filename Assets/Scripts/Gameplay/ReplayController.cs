using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.GameStateManagement;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Timing;

namespace UStacker.Gameplay
{
    public class ReplayController : MonoBehaviour
    {
        [SerializeField] private Mediator _mediator;
        [SerializeField] private GameStateManager _stateManager;
        [Header("Controlled objects")]
        [SerializeField] private GameTimer _timer;
        [SerializeField] private InputProcessor _inputProcessor;

        [Header("Controls")]
        [SerializeField] private Slider _timeSlider;
        [SerializeField] private Image _pauseImage;
        [SerializeField] private Button _endReplayButton;
        [SerializeField] private Button _nextPieceButton;
        [SerializeField] private Button _prevPieceButton;
        [SerializeField] private Button _tenthSecondForwardButton;
        [SerializeField] private Button _tenthSecondBackwardButton;
        [SerializeField] private Button _fiveSecondsForwardButton;
        [SerializeField] private Button _fiveSecondsBackwardButton;
        [SerializeField] private TMP_Text _currentTimeText;
        [SerializeField] private TMP_InputField _timeScaleField;

        [Header("Used values")]
        [SerializeField] private Sprite _pauseSprite;
        [SerializeField] private Sprite _playSprite;

        private double _replayLength;

        private void Awake()
        {
            _timeSlider.onValueChanged.AddListener(OnTimeSet);
            
            _endReplayButton.onClick.AddListener(() => _stateManager.EndReplay(_replayLength));
            _nextPieceButton.onClick.AddListener(_inputProcessor.MoveToNextPiece);
            _prevPieceButton.onClick.AddListener(_inputProcessor.MoveToPrevPiece);
            _tenthSecondForwardButton.onClick.AddListener(_inputProcessor.MoveTenthSecondForward);
            _tenthSecondBackwardButton.onClick.AddListener(_inputProcessor.MoveTenthSecondBackward);
            _fiveSecondsForwardButton.onClick.AddListener(_inputProcessor.MoveFiveSecondsForward);
            _fiveSecondsBackwardButton.onClick.AddListener(_inputProcessor.MoveFiveSecondsBackward);
            _timeScaleField.onEndEdit.AddListener(OnSpeedChanged);
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            _pauseImage.sprite = message.NewState switch
            {
                GameState.Paused => _playSprite,
                GameState.Running => _pauseSprite,
                _ => _pauseImage.sprite
            };
        }

        private void OnEnable()
        {
            _timeScaleField.text = _timer.TimeScale.ToString(CultureInfo.InvariantCulture);
        }

        private void Update()
        {
            _currentTimeText.text = _timer.CurrentTime.FormatAsTime();
            _timeSlider.SetValueWithoutNotify((float)_timer.CurrentTime);
        }

        private void OnSpeedChanged(string timeScaleStr)
        {
            if (!timeScaleStr.TryParseDouble(out var newTimeScale))
            {
                _timeScaleField.text = _timer.TimeScale.ToString(CultureInfo.InvariantCulture);
                return;
            }

            newTimeScale = Math.Max(newTimeScale, 0.01);
            _timer.TimeScale = newTimeScale;
            _timeScaleField.text = _timer.TimeScale.ToString(CultureInfo.InvariantCulture);
        }

        private void OnTimeSet(float newTime)
        {
            _timer.SetTime(newTime);
        }

        public void SetReplay(GameReplay replay)
        {
            _timeSlider.SetValueWithoutNotify(0);
            _replayLength = replay.GameLength;
            _timeSlider.maxValue = (float) replay.GameLength - float.Epsilon;
        }
    }
}