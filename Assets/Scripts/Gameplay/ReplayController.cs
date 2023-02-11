using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Extensions;
using UStacker.Gameplay.InputProcessing;
using UStacker.Gameplay.Timing;

namespace UStacker.Gameplay
{
    public class ReplayController : MonoBehaviour
    {
        [Header("Controlled objects")]
        [SerializeField] private GameTimer _timer;
        [SerializeField] private InputProcessor _inputProcessor;
        [SerializeField] private GameStateManager _stateManager;

        [Header("Controls")]
        [SerializeField] private Slider _timeSlider;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Image _pauseImage;
        [SerializeField] private Button _nextPieceButton;
        [SerializeField] private Button _prevPieceButton;
        [SerializeField] private Button _nextActionButton;
        [SerializeField] private Button _prevActionButton;
        [SerializeField] private TMP_Text _currentTimeText;
        [SerializeField] private TMP_InputField _timeScaleField;

        [Header("Used values")]
        [SerializeField] private Sprite _pauseSprite;
        [SerializeField] private Sprite _playSprite;

        private void Awake()
        {
            _timeSlider.onValueChanged.AddListener(OnTimeSet);
            _pauseButton.onClick.AddListener(TogglePause);
            _nextPieceButton.onClick.AddListener(_inputProcessor.MoveToNextPiece);
            _prevPieceButton.onClick.AddListener(_inputProcessor.MoveToPrevPiece);
            _nextActionButton.onClick.AddListener(_inputProcessor.MoveToNextAction);
            _prevActionButton.onClick.AddListener(_inputProcessor.MoveToPrevAction);
            _timeScaleField.onEndEdit.AddListener(OnSpeedChanged);
            _stateManager.ReplayPaused.AddListener(() => _pauseImage.sprite = _playSprite);
            _stateManager.ReplayUnpaused.AddListener(() => _pauseImage.sprite = _pauseSprite);
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

        private void TogglePause()
        {
            _stateManager.TogglePause();
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
            _timeSlider.maxValue = (float) replay.GameLength - float.Epsilon;
        }
    }
}