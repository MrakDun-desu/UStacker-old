using Blockstacker.Gameplay.Communication;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Presentation
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameCountdown : MonoBehaviour
    {
        public float CountdownInterval = .1f;
        public uint CountdownCount = 3;
        [SerializeField] private string _noCountdownMessage = "Ready";
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private MediatorSO _mediator;
        [SerializeField] private UnityEvent CountdownFinished;
        private bool _active;

        private TMP_Text _countdownText;
        private uint _currentCount;
        private float _nextInterval;

        private void Awake()
        {
            _countdownText = GetComponent<TMP_Text>();
            _mediator.Register<GameLostMessage>(_ => StopCountdown());
            _mediator.Register<GamePausedMessage>(_ => StopCountdown());
            _mediator.Register<GameResumedMessage>(_ => RestartCountdown());
            _mediator.Register<GameRestartedMessage>(_ => RestartCountdown());
        }

        private void Update()
        {
            if (!_active) return;

            while (Time.realtimeSinceStartup >= _nextInterval)
            {
                _currentCount--;
                _nextInterval += CountdownInterval;

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
                    _mediator.Send(new CountdownTickedMessage(_currentCount - 1));
            }
        }

        private void StopCountdown()
        {
            _active = false;
        }

        private void RestartCountdown()
        {
            if (_currentCount == 0) return;
            StopCountdown();
            StartCountdown();
        }

        public void StartCountdown()
        {
            _countdownText.gameObject.SetActive(true);
            _active = true;
            _nextInterval = Time.realtimeSinceStartup + CountdownInterval;
            _currentCount = CountdownCount + 1;
            _countdownText.text = _currentCount == 2 ? _noCountdownMessage : CountdownCount.ToString();
            _mediator.Send(new CountdownTickedMessage(_currentCount + 1));
        }
    }
}