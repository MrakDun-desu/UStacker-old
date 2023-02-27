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
        [SerializeField] private string _noCountdownMessage = "Ready";
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private Mediator _mediator;
        [SerializeField] private UnityEvent CountdownFinished;
        
        public GameSettingsSO.SettingsContainer GameSettings { private get; set; }
        
        private bool _active;
        private TMP_Text _countdownText;
        private uint _currentCount;
        private float _nextInterval;
        private float _interval = .1f;
        private uint _count = 3;

        private void Awake()
        {
            _countdownText = GetComponent<TMP_Text>();
            CountdownFinished.AddListener(() => gameObject.SetActive(false));
            _mediator.Register<GameStateChangedMessage>(OnGameStateChange);
            
            FirstTimeInitialize();
        }

        private void FirstTimeInitialize()
        {
            _gameTitle.text = GameSettings.Presentation.Title;
            _interval = GameSettings.Presentation.CountdownInterval;
            _count = GameSettings.Presentation.CountdownCount;
        }
        
        private void OnGameStateChange(GameStateChangedMessage message)
        {
            if (message.NewState is GameState.GameStartCountdown or GameState.GameResumeCountdown)
            {
                if (message.IsReplay)
                    CountdownFinished.Invoke();
                else
                    StartCountdown();
            }
            else if (message.PreviousState is GameState.GameStartCountdown or GameState.GameResumeCountdown)
                StopCountdown();
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
            _countdownText.gameObject.SetActive(false);
        }

        private void StartCountdown()
        {
            _countdownText.gameObject.SetActive(true);
            _nextInterval = Time.realtimeSinceStartup + _interval;
            _active = true;
            _currentCount = _count + 1;
            _countdownText.text = _currentCount == 2 ? _noCountdownMessage : _count.ToString();
            _mediator.Send(new CountdownTickedMessage(_currentCount));
        }
    }
}