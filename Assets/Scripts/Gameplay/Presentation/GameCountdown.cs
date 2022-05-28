using System;
using System.Collections;
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
        [SerializeField] private string _lastMessage = "Start!";
        [SerializeField] private GameManager _manager;
        [SerializeField] private UnityEvent CountdownFinished;

        private TMP_Text _countdownText;
        private bool _active;
        private uint _currentCount;
        private float _nextInterval;

        private void Awake()
        {
            _countdownText = GetComponent<TMP_Text>();
            _manager.GameEndedEvent += StopCountdown;
            _manager.GamePausedEvent += StopCountdown;
            _manager.GameResumedEvent += RestartCountdown;
            _manager.GameRestartedEvent += RestartCountdown;
        }

        private void StopCountdown()
        {
            _active = false;
        }

        private void RestartCountdown()
        {
            StopCountdown();
            StartCountdown();
        }

        public void StartCountdown()
        {
            _countdownText.gameObject.SetActive(true);
            _active = true;
            _nextInterval = Time.realtimeSinceStartup + CountdownInterval;
            _currentCount = CountdownCount + 1;
            _countdownText.text = CountdownCount.ToString();
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
            }
        }
    }
}