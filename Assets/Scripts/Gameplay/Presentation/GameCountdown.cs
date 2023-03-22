using UStacker.Gameplay.Communication;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay.Presentation
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameCountdown : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private TMP_Text _gameTitle;
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private string _noCountdownMessage = "Ready";
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private Mediator _mediator;
        [SerializeField] private UnityEvent CountdownFinished;

        private GameSettingsSO.SettingsContainer _gameSettings;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            private get => _gameSettings;
            set
            {
                _gameSettings = value;
                Initialize();
            }
        }

        private bool _active;
        private uint _currentCount;
        private float _nextInterval;
        private float _interval = .1f;
        private uint _count = 3;

        private void Awake()
        {
            CountdownFinished.AddListener(() => _countdownText.alpha = 0);
        }

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChange);
        }

        private void Initialize()
        {
            _gameTitle.text = GameSettings.Presentation.Title;
            _interval = GameSettings.Presentation.CountdownInterval;
            _count = GameSettings.Presentation.CountdownCount;

            transform.localPosition = new Vector2(
                GameSettings.BoardDimensions.BoardWidth / 2f,
                GameSettings.BoardDimensions.BoardHeight / 2f);
        }

        private void OnGameStateChange(GameStateChangedMessage message)
        {
            switch (message)
            {
                case {NewState: GameState.ResumeCountdown, IsReplay: true}:
                    CountdownFinished.Invoke();
                    break;
                case {NewState: GameState.StartCountdown or GameState.ResumeCountdown}:
                    StartCountdown();
                    break;
                case {PreviousState: GameState.StartCountdown or GameState.ResumeCountdown}:
                    StopCountdown();
                    break;
            }
        }

        private void Update()
        {
            if (!_active) return;

            while (Time.realtimeSinceStartup >= _nextInterval)
            {
                _currentCount--;
                _nextInterval += _interval;

                switch (_currentCount)
                {
                    case > 1:
                        _countdownText.text = (_currentCount - 1).ToString();
                        break;
                    case 1:
                        _countdownText.text = _lastMessage;
                        break;
                    case 0:
                        _active = false;
                        CountdownFinished.Invoke();
                        break;
                }

                if (_active)
                    _mediator.Send(new CountdownTickedMessage(_currentCount));
            }
        }

        private void StopCountdown()
        {
            _active = false;
            _countdownText.alpha = 0;
        }

        private void StartCountdown()
        {
            _countdownText.alpha = 1;
            _nextInterval = Time.realtimeSinceStartup + _interval;
            _active = true;
            _currentCount = _count + 1;
            _countdownText.text = _currentCount == 2 ? _noCountdownMessage : _count.ToString();
            _mediator.Send(new CountdownTickedMessage(_currentCount));
        }
    }
}