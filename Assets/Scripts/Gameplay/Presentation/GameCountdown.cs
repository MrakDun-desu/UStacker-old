using UStacker.Gameplay.Communication;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UStacker.Gameplay.Initialization;

namespace UStacker.Gameplay.Presentation
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameCountdown : MonoBehaviour, IMediatorDependency
    {
        public float CountdownInterval = .1f;
        public uint CountdownCount = 3;
        [SerializeField] private string _noCountdownMessage = "Ready";
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private UnityEvent CountdownFinished;
        private bool _active;

        private TMP_Text _countdownText;
        private uint _currentCount;
        private float _nextInterval;
        public Mediator Mediator { private get; set; }

        private void Awake()
        {
            _countdownText = GetComponent<TMP_Text>();
            CountdownFinished.AddListener(() => gameObject.SetActive(false));
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
                    Mediator.Send(new CountdownTickedMessage(_currentCount));
            }
        }

        public void StopCountdown()
        {
            _active = false;
        }

        public void StartCountdown()
        {
            _countdownText.gameObject.SetActive(true);
            _nextInterval = Time.realtimeSinceStartup + CountdownInterval;
            _active = true;
            _currentCount = CountdownCount + 1;
            _countdownText.text = _currentCount == 2 ? _noCountdownMessage : CountdownCount.ToString();
            Mediator.Send(new CountdownTickedMessage(_currentCount));
        }
    }
}